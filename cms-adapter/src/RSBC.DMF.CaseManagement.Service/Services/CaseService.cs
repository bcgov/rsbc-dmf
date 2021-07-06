using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace RSBC.DMF.CaseManagement.Service
{
    //[Authorize]
    public class CaseService : CaseManager.CaseManagerBase
    {
        private readonly ILogger<CaseService> _logger;
        private readonly ICaseManager caseManager;

        public CaseService(ILogger<CaseService> logger, ICaseManager caseManager)
        {
            _logger = logger;
            this.caseManager = caseManager;
        }

        public async override Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            var searchResult = await caseManager.CaseSearch(new CaseSearchRequest { ByCaseId = request.CaseId });

            var reply = new SearchReply();
            reply.Items.Add(searchResult.Items.Select(c => new Case
            {
                CaseId = c.Id,
                CreatedBy = c.CreatedBy,
                CreatedOn = Timestamp.FromDateTime(c.CreatedOn)
            }));

            return reply;
        }
    }
}