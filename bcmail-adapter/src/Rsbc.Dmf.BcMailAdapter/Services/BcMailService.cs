using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rsbc.Interfaces;
using System.Threading.Tasks;
using Rsbc.Dmf.CaseManagement.Service;
using static Rsbc.Dmf.CaseManagement.Service.CaseManager;
using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
using Pssg.DocumentStorageAdapter;
using Hangfire.Server;

namespace Rsbc.Dmf.BcMailAdapter.Services
{
    /// <summary>
    /// BcMailService
    /// </summary>
    public class BcMailService
    {
        private readonly ILogger<BcMailService> _logger;
        private readonly IConfiguration _configuration;        
        private readonly CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapterClient _documentStorageAdapterClient;
       
        /// <summary>
        /// BcMailService
        /// </summary>
        /// <param name="logger"></param>      
        /// <param name="configuration"></param>
        public BcMailService(ILogger<BcMailService> logger, IConfiguration configuration, CaseManagerClient caseManagerClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
        {
            _configuration = configuration;
            _logger = logger;
            _caseManagerClient = caseManagerClient;
            _documentStorageAdapterClient = documentStorageAdapterClient;
          
        }

        /// <summary>
        /// SendDocumentsToBcMail
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResultStatusReply> SendDocumentsToBcMail(EmptyRequest request, PerformContext hangfireContext)
        {
            var result = new ResultStatusReply();

            var sfegUtils = new SfegUtils(_configuration, _caseManagerClient, _documentStorageAdapterClient);
            sfegUtils.SendDocumentsToBcMail(hangfireContext).GetAwaiter().GetResult();
            return result; 

        }
    }
}
