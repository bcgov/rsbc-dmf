using RSBC.DMF.CaseManagement.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSBC.DMF.CaseManagement
{
    public interface ICaseManager
    {
        Task<CaseSearchReply> CaseSearch(CaseSearchRequest request);
    }

    public class CaseSearchRequest
    {
        public string ByCaseId { get; set; }
    }

    public class CaseSearchReply
    {
        public IEnumerable<Case> Items { get; set; }
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
    }
}