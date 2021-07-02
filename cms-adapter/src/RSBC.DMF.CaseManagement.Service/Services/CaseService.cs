using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RSBC.DMF.CaseManagement.Service
{
    [Authorize]
    public class CaseService : CaseManager.CaseManagerBase
    {
        private readonly ILogger<CaseService> _logger;

        public CaseService(ILogger<CaseService> logger)
        {
            _logger = logger;
        }

        public async override Task<DmerReply> SubmitDmer(DmerRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new DmerReply { CaseId = request.CaseId });
        }
    }
}