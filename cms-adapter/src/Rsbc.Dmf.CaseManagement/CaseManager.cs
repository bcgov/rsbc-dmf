using Microsoft.Extensions.Logging;
using Microsoft.OData.Client;
using Rsbc.Dmf.CaseManagement.Dynamics;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement
{
    public interface ICaseManager
    {
        Task<CaseSearchReply> CaseSearch(CaseSearchRequest request);

        Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, ILogger logger = null);
    }

    public class CaseSearchRequest
    {
        public string CaseId { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string ClinicId { get; set; }
    }

    public class CaseSearchReply
    {
        public IEnumerable<Case> Items { get; set; }
    }

    public class SetCaseFlagsReply
    {
        public bool Success { get; set; }
    }

    public abstract class Case
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverName { get; set; }
    }

    public class DmerCase : Case
    {
        public IEnumerable<Flag> Flags { get; set; }
        public string ClinicId { get; set; }
        public string ClinicName { get; set; }
    }

    public class Flag
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    internal class CaseManager : ICaseManager
    {
        private readonly DynamicsContext dynamicsContext;
        private readonly ILogger<CaseManager> logger;

        public CaseManager(DynamicsContext dynamicsContext, ILogger<CaseManager> logger)
        {
            this.dynamicsContext = dynamicsContext;
            this.logger = logger;
        }

        public async Task<CaseSearchReply> CaseSearch(CaseSearchRequest request)
        {
            //search matching cases
            var cases = (await SearchCases(dynamicsContext, request)).Concat(await SearchDriverCases(dynamicsContext, request));

            //lazy load case related properties
            foreach (var @case in cases)
            {
                //load clinic details (assuming customer as clinic for now)
                if (@case.customerid_contact == null) await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.customerid_contact));

                if (@case._dfp_driverid_value.HasValue)
                {
                    //load driver info
                    await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_DriverId));
                    if (@case.dfp_DriverId != null) await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId, nameof(incident.dfp_DriverId.dfp_PersonId));
                }

                //load case's flags
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_dmerflag));
                foreach (var flag in @case.dfp_incident_dfp_dmerflag)
                {
                    await dynamicsContext.LoadPropertyAsync(flag, nameof(dfp_dmerflag.dfp_FlagId));
                }
            }

            dynamicsContext.DetachAll();

            //map cases from query results (TODO: consider replacing with AutoMapper)
            return new CaseSearchReply
            {
                Items = cases.Select(c => new DmerCase
                {
                    Id = c.title,
                    CreatedBy = $"{c.customerid_contact?.lastname?.ToUpper()}, {c.customerid_contact?.firstname}",
                    CreatedOn = c.createdon.Value.DateTime,
                    ModifiedBy = $"{c.customerid_contact?.lastname?.ToUpper()}, {c.customerid_contact?.firstname}",
                    ModifiedOn = c.modifiedon.Value.DateTime,
                    DriverLicenseNumber = c.dfp_DriverId?.dfp_licensenumber,
                    DriverName = $"{c.dfp_DriverId?.dfp_PersonId?.lastname.ToUpper()}, {c.dfp_DriverId?.dfp_PersonId?.firstname}",
                    ClinicId = c.customerid_contact.contactid.ToString(),
                    ClinicName = $"{c.customerid_contact?.firstname} {c.customerid_contact?.lastname}",
                    Flags = c.dfp_incident_dfp_dmerflag
                        .Where(f => f.dfp_FlagId != null) //temp defense against deleted flags
                        .Select(f => new Flag
                        {
                            Id = f.dfp_FlagId?.dfp_id,
                            Description = f.dfp_FlagId?.dfp_description
                        }).ToArray()
                }).ToArray()
            };
        }

        private static async Task<IEnumerable<incident>> SearchCases(DynamicsContext ctx, CaseSearchRequest criteria)
        {
            var shouldSearchCases =
                !string.IsNullOrEmpty(criteria.CaseId) ||
                !string.IsNullOrEmpty(criteria.ClinicId);

            if (!shouldSearchCases) return Array.Empty<incident>();

            var caseQuery = ctx.incidents
                .Expand(i => i.dfp_DriverId)
                .Expand(i => i.customerid_contact)
                .Where(i => i.casetypecode == (int)CaseTypeOptionSet.DMER);

            if (!string.IsNullOrEmpty(criteria.CaseId)) caseQuery = caseQuery.Where(i => i.title == criteria.CaseId);
            if (!string.IsNullOrEmpty(criteria.ClinicId)) caseQuery = caseQuery.Where(i => i._customerid_value == Guid.Parse(criteria.ClinicId));

            return (await ((DataServiceQuery<incident>)caseQuery).GetAllPagesAsync()).ToArray();
        }

        private static async Task<IEnumerable<incident>> SearchDriverCases(DynamicsContext ctx, CaseSearchRequest criteria)
        {
            var shouldSearchDrivers = !string.IsNullOrEmpty(criteria.DriverLicenseNumber);

            if (!shouldSearchDrivers) return Array.Empty<incident>();

            var driverQuery = ctx.dfp_drivers.Expand(d => d.dfp_PersonId).Where(d => d.statecode == (int)EntityState.Active);
            if (!string.IsNullOrEmpty(criteria.DriverLicenseNumber)) driverQuery = driverQuery.Where(i => i.dfp_licensenumber == criteria.DriverLicenseNumber);
            var drivers = (await ((DataServiceQuery<dfp_driver>)driverQuery).GetAllPagesAsync()).ToArray();
            foreach (var driver in drivers)
            {
                await ctx.LoadPropertyAsync(driver, nameof(dfp_driver.dfp_driver_incident_DriverId));
            }

            return drivers.SelectMany(d => d.dfp_driver_incident_DriverId).ToArray();
        }

        public async Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, ILogger logger = null)
        {
            if (logger == null) logger = this.logger;
            /* The structure for cases is

            Case (incident) is the parent item
                - has children which are flag entities

             */

            logger.LogInformation($"SetCaseFlags - looking for DMER with identifier {dmerIdentifier}");

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.Where(x => x.title == dmerIdentifier).FirstOrDefault();

            if (dmerEntity != null)
            {
                // close and re-open the case
                dynamicsContext.CloseIncident(new incidentresolution()
                {
                    incidentid = dmerEntity
                }, -1);

                dmerEntity.statecode = 0;
                dmerEntity.statuscode = 1;

                // clean pass is indicated by the presence of flags.
                logger.LogInformation($"SetCaseFlags - found DMER with identifier {dmerIdentifier}");

                // Explicitly load the flags
                dynamicsContext.LoadProperty(dmerEntity, nameof(incident.dfp_incident_dfp_dmerflag));

                foreach (var item in dmerEntity.dfp_incident_dfp_dmerflag)
                {
                    // remove the old bridge.
                    dynamicsContext.DeleteObject(item);

                    //dmerEntity.dfp_incident_dfp_flag.
                    logger.LogInformation($"SetCaseFlags - removing flag {item.dfp_name}");
                }

                // Add the flags.

                foreach (var flag in flags)
                {
                    dfp_flag givenFlag = dynamicsContext.dfp_flags.Where(x => x.dfp_id == flag.Id).FirstOrDefault();
                    if (givenFlag == null)
                    {
                        givenFlag = new dfp_flag()
                        {
                            dfp_id = flag.Id,
                            dfp_description = flag.Description
                        };
                        dynamicsContext.AddTodfp_flags(givenFlag);
                    }

                    // configure the bridge entity

                    dfp_dmerflag newFlag = new dfp_dmerflag()
                    {
                        dfp_name = flag.Description
                    };

                    dynamicsContext.AddTodfp_dmerflags(newFlag);
                    dynamicsContext.AddLink(dmerEntity, nameof(incident.dfp_incident_dfp_dmerflag), newFlag);
                    dynamicsContext.SetLink(newFlag, nameof(dfp_dmerflag.dfp_FlagId), givenFlag);

                    logger.LogInformation($"SetCaseFlags - Added Flag {flag}");
                }

                // update the case from the request.

                //dmerEntity.modifiedon = DateTimeOffset.Now;

                // indicate that the form has been filled out
                //dmerEntity.statuscode = 4; // Researching - was // 100000003; // Completed

                dmerEntity.dfp_iscleanpass = isCleanPass;

                dynamicsContext.UpdateObject(dmerEntity);

                try
                {
                    await dynamicsContext.SaveChangesAsync();
                    dynamicsContext.DetachAll();

                    return new SetCaseFlagsReply { Success = true };
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"SetCaseFlags - Error updating");
                }
            }
            else
            {
                logger.LogError($"SetCaseFlags - Unable to find DMER with identifier {dmerIdentifier}");
            }

            dynamicsContext.DetachAll();

            return new SetCaseFlagsReply { Success = false };
        }
    }

    internal enum CaseTypeOptionSet
    {
        DMER = 2
    }
}