using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pssg.DocumentStorageAdapter;
using Rsbc.Dmf.CaseManagement.Service;
using Rsbc.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
//using static Pssg.DocumentStorageAdapter.DocumentStorageAdapter;
//using static Rsbc.Dmf.CaseManagement.Service.CaseManager;


namespace Rsbc.Dmf.BcMailAdapter.Services
{
    /// <summary>
    /// BcMailService
    /// </summary>
    [Authorize]
    public class BcMailService: BcMailAdapter.BcMailAdapterBase
    {
        private readonly ILogger<BcMailService> _logger;
        private readonly IConfiguration _configuration;        
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly DocumentStorageAdapter.DocumentStorageAdapterClient _documentStorageAdapterClient;

       
        /// <summary>
        /// BcMailService
        /// </summary>
        /// <param name="logger"></param>      
        /// <param name="configuration"></param>
        public BcMailService(ILogger<BcMailService> logger, IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, DocumentStorageAdapter.DocumentStorageAdapterClient documentStorageAdapterClient)
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
        public override Task<ResultStatusReply> SendDocumentsToBcMail(EmptyRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            var sfegUtils = new SfegUtils(_configuration, _caseManagerClient);
            sfegUtils.SendDocumentsToBcMail();
            return Task.FromResult(result);

        }

        /// <summary>
        /// Get Token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            var result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            var configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(request.Secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: creds
                );
                result.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                result.ResultStatus = ResultStatus.Success;
            }
            else
            {
                result.ErrorDetail = "Bad Request";
            }

            return Task.FromResult(result);
        }


    
}
}
