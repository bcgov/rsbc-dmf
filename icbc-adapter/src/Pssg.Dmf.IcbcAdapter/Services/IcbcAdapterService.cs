using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pssg.Interfaces;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;

namespace Rsbc.Dmf.IcbcAdapter.Services
{
    // Default to require authorization
    [Authorize]
    public class IcbcAdapterService : IcbcAdapter.IcbcAdapterBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IcbcAdapterService> _logger;
        private readonly CaseManager.CaseManagerClient _caseManagerClient;
        private readonly IIcbcClient _icbcClient;


        public IcbcAdapterService(ILogger<IcbcAdapterService> logger, IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient, IIcbcClient icbcClient)
        {
            _configuration = configuration;
            _logger = logger;
            _caseManagerClient = caseManagerClient;
            _icbcClient = icbcClient;
        }

        public override Task<DriverInfoReply> GetDriverInfo(DriverInfoRequest request, ServerCallContext context)
        {
            var result = new DriverInfoReply();
            
            // call the client

            return Task.FromResult(result);
        }

        public override Task<ResultStatusReply> ProcessMedicalStatusUpdates(EmptyRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            // process medical status updates

            var enhancedIcbcUtils = new EnhancedIcbcApiUtils(_configuration, _caseManagerClient, _icbcClient);
                enhancedIcbcUtils.SendMedicalUpdates(null).GetAwaiter().GetResult();

            return Task.FromResult(result);
        }


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