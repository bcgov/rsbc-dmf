using Rsbc.Dmf.CaseManagement.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Client;
using Rsbc.Dmf.Dynamics.Microsoft.Dynamics.CRM;
using Microsoft.Extensions.Logging;

namespace Rsbc.Dmf.CaseManagement
{
    public interface ICaseManager
    {
        Task<CaseSearchReply> CaseSearch(CaseSearchRequest request);

        Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, bool isCleanPass,  List<Flag> flags, ILogger logger = null);
    }

    public class CaseSearchRequest
    {
        public string ByCaseId { get; set; }
    }

    public class CaseSearchReply
    {
        public IEnumerable<Case> Items { get; set; }
    }

    public class SetCaseFlagsReply
    {
        public bool Success { get; set; }
    }
    
    public class Case
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
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
            var cases = await dynamicsContext.tasks.GetAllPagesAsync();

            return new CaseSearchReply
            {
                Items = cases.Select(c => new Case
                {
                    Id = c.activityid.ToString(),
                    CreatedBy = c.createdby.identityid?.ToString(),
                    CreatedOn = c.createdon.Value.DateTime
                }).ToArray()
            };
        }

        public async Task<SetCaseFlagsReply>SetCaseFlags(string dmerIdentifier, bool isCleanPass, List<Flag> flags, ILogger logger = null)
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

                dynamicsContext.SaveChanges();


                dynamicsContext.UpdateObject(dmerEntity);

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
}