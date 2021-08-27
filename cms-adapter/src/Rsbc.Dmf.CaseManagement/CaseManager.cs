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
        public string DriverLicenseNumber { get; set; }
        public string DriverName { get; set; }
    }

    public class DmerCase : Case
    {
        public IEnumerable<Flag> Flags { get; set; }
    }

    public class Flag
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    internal class CaseManager : ICaseManager
    {
        private readonly DynamicsContext dynamicsContext;

        public CaseManager(DynamicsContext dynamicsContext)
        {
            this.dynamicsContext = dynamicsContext;
        }

        public async Task<CaseSearchReply> CaseSearch(CaseSearchRequest request)
        {
            var caseQuery = dynamicsContext.incidents
                .Expand(i => i.dfp_DriverId)
                .Where(i => i.casetypecode == (int)CaseTypeOptionSet.DMER);

            if (!string.IsNullOrEmpty(request.CaseId)) caseQuery = caseQuery.Where(i => i.title == request.CaseId);
            if (!string.IsNullOrEmpty(request.DriverLicenseNumber)) caseQuery = caseQuery.Where(i => i.dfp_DriverId.dfp_licensenumber == request.DriverLicenseNumber);

            var cases = (await ((DataServiceQuery<incident>)caseQuery).GetAllPagesAsync()).ToArray();

            //lazy load case related properties
            foreach (var @case in cases)
            {
                await dynamicsContext.LoadPropertyAsync(@case, nameof(incident.dfp_incident_dfp_dmerflag));
                if (@case.dfp_DriverId != null) await dynamicsContext.LoadPropertyAsync(@case.dfp_DriverId, nameof(incident.dfp_DriverId.dfp_PersonId));
            }

            return new CaseSearchReply
            {
                //map cases from query results (TODO: consider replacing with AutoMapper)
                Items = cases.Select(c => new DmerCase
                {
                    Id = c.title,
                    //TODO: CreatedBy =
                    CreatedOn = c.createdon.Value.DateTime,
                    DriverLicenseNumber = c.dfp_DriverId?.dfp_licensenumber,
                    DriverName = $"{c.dfp_DriverId?.dfp_PersonId?.lastname.ToUpper()}, {c.dfp_DriverId?.dfp_PersonId?.firstname}",
                    Flags = c.dfp_incident_dfp_dmerflag.Select(f => new Flag { Id = f._dfp_flagid_value?.ToString(), Description = f.dfp_name }).ToArray()
                }).ToArray()
            };
        }

        public async Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, ILogger logger = null)
        {
            /* The structure for cases is

            Case (incident) is the parent item
                - has children which are flag entities

             */

            var result = new SetCaseFlagsReply()
            {
                Success = false
            };

            if (logger != null)
            {
                logger.LogInformation($"SetCaseFlags - looking for DMER with identifier {dmerIdentifier}");
            }

            // future state - the case name will contain three letters of the name and the driver licence number

            incident dmerEntity = dynamicsContext.incidents.Where(x => x.title == dmerIdentifier).FirstOrDefault();

            if (dmerEntity != null)
            {
                // close and re-open the case
                dynamicsContext.CloseIncident(new incidentresolution()
                {
                    incidentid = dmerEntity
                }, -1);
                dynamicsContext.SaveChanges();

                dmerEntity.statecode = 0;
                dmerEntity.statuscode = 1;
                dynamicsContext.UpdateObject(dmerEntity);
                dynamicsContext.SaveChanges();

                // clean pass is indicated by the presence of flags.
                if (logger != null)
                {
                    logger.LogInformation($"SetCaseFlags - found DMER with identifier {dmerIdentifier}");
                }

                // Explicitly load the flags
                dynamicsContext.LoadProperty(dmerEntity, "dfp_incident_dfp_dmerflag");

                // replace with unlink.  may need to replace with lazy loading
                if (dmerEntity.dfp_incident_dfp_dmerflag != null && dmerEntity.dfp_incident_dfp_dmerflag.Count > 0)
                {
                    foreach (var item in dmerEntity.dfp_incident_dfp_dmerflag)
                    {
                        //dynamicsContext.DeleteLink(dmerEntity, "dfp_incident_dfp_dmerflag", item);
                        //dynamicsContext.SaveChanges();

                        // remove the old bridge.
                        dynamicsContext.DeleteObject(item);
                        dynamicsContext.SaveChanges();

                        //dmerEntity.dfp_incident_dfp_flag.
                        logger.LogInformation($"SetCaseFlags - removing flag {item.dfp_name}");
                    }
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
                        dynamicsContext.SaveChanges();
                    }

                    // configure the bridge entity

                    dfp_dmerflag newFlag = new dfp_dmerflag()
                    {
                        dfp_name = flag.Description
                    };

                    dynamicsContext.AddTodfp_dmerflags(newFlag);
                    dynamicsContext.SaveChanges();

                    dynamicsContext.UpdateRelatedObject(newFlag, "dfp_FlagId", givenFlag);
                    dynamicsContext.SaveChanges();

                    dynamicsContext.AddLink(dmerEntity, "dfp_incident_dfp_dmerflag", newFlag);
                    dynamicsContext.SaveChanges();
                    if (logger != null)
                    {
                        logger.LogInformation($"SetCaseFlags - Added Flag {flag}");
                    }
                }

                // update the case from the request.

                dmerEntity.modifiedon = DateTimeOffset.Now;

                // indicate that the form has been filled out
                //dmerEntity.statuscode = 4; // Researching - was // 100000003; // Completed

                dmerEntity.dfp_iscleanpass = isCleanPass;

                dynamicsContext.UpdateObject(dmerEntity);
                try
                {
                    DataServiceResponse dsr = dynamicsContext.SaveChanges();
                    result.Success = true;
                }
                catch (Exception e)
                {
                    if (logger != null)
                    {
                        logger.LogInformation(e, $"SetCaseFlags - Error updating");
                    }
                }
            }
            else
            {
                if (logger != null)
                {
                    logger.LogInformation($"SetCaseFlags - Unable to find DMER with identifier {dmerIdentifier}");
                }
            }

            return result;
        }
    }

    internal enum CaseTypeOptionSet
    {
        DMER = 2
    }
}