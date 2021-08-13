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

        Task<SetCaseFlagsReply> SetCaseFlags(string dmerIdentifier, List<string> flags, ILogger logger = null);
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

        public async Task<SetCaseFlagsReply>SetCaseFlags(string dmerIdentifier, List<string> flags, ILogger logger = null)
        {
            /* The structure for cases is
             
            Case (incident) is the parent item
                - has a child which is a dmer entity
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

            dfp_dmer dmerEntity = dynamicsContext.dfp_dmers.Where(x => x.dfp_id == dmerIdentifier).First();

            if (dmerEntity != null)
            {
                // clean pass is indicated by the precense of flags.  
                if (logger != null)
                {
                    logger.LogInformation($"SetCaseFlags - found DMER with identifier {dmerIdentifier}");
                }
                
                dmerEntity.dfp_dmer_dfp_flag.Clear();

                // Add the flags.

                foreach (var flag in flags)
                {
                    dfp_flag newFlag = new dfp_flag()
                    {
                    };
                    newFlag.dfp_question = flag;
                    dmerEntity.dfp_dmer_dfp_flag.Add(newFlag);
                    if (logger != null)
                    {
                        logger.LogInformation($"SetCaseFlags - Added Flag {flag}");
                    }
                }

                // update the case from the request.

                dmerEntity.modifiedon = DateTimeOffset.Now;

                // indicate that the form has been filled out
                dmerEntity.statuscode = 100000003; // Completed

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