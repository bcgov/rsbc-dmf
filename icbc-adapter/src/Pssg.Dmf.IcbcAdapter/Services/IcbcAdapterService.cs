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

        /// <summary>
        /// Get Driver Info
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<DriverInfoReply> GetDriverInfo(DriverInfoRequest request, ServerCallContext context)
        {
            var result = new DriverInfoReply();
            
            var reply = _icbcClient.GetDriverHistory(request.DriverLicence);
            if (reply != null)
            {
                result.AddressLine1 = reply.ToAddressLine1();
                result.BirthDate = reply.BIDT?.ToString() ?? string.Empty;
                result.City = reply.ADDR?.CITY ?? string.Empty;
                result.GivenName = reply.INAM?.GIV1 ?? string.Empty;
                result.Postal = reply.ADDR?.POST ?? string.Empty;
                result.Province = reply.ADDR?.PROV ?? string.Empty;
                result.Country = reply.ADDR?.CNTY ?? string.Empty;
                result.Sex = reply.SEX ?? string.Empty;
                result.Surname = reply.INAM?.SURN ?? string.Empty;
                result.LicenceClass = reply.DR1MST?.LCLS ?? default;
                result.ResultStatus = ResultStatus.Success;
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// Process Medical Status Updates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ResultStatusReply> ProcessMedicalStatusUpdates(EmptyRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            // process medical status updates

            var enhancedIcbcUtils = new EnhancedIcbcApiUtils(_configuration, _caseManagerClient, _icbcClient);
                enhancedIcbcUtils.SendMedicalUpdates().GetAwaiter().GetResult();

            return Task.FromResult(result);
        }

        public override Task<ResultStatusReply> DryRunMedicalDisposition(EmptyRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            // process medical status updates - dry run

            var enhancedIcbcUtils = new EnhancedIcbcApiUtils(_configuration, _caseManagerClient, _icbcClient);
            enhancedIcbcUtils.SendMedicalUpdatesDryRun().GetAwaiter().GetResult();

            return Task.FromResult(result);
        }

        

        /// <summary>
        /// Resolve Birth Dates
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ResultStatusReply> UpdateBirthdate(EmptyRequest request, ServerCallContext context)
        {
            var result = new ResultStatusReply();

            var enhancedIcbcUtils = new EnhancedIcbcApiUtils(_configuration, _caseManagerClient, _icbcClient);
            enhancedIcbcUtils.UpdateBirthdateFromIcbc().GetAwaiter().GetResult();

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
                byte[] secretBytes = Encoding.UTF8.GetBytes(_configuration["JWT_TOKEN_KEY"]);
                Array.Resize(ref secretBytes, 32);

                var key = new SymmetricSecurityKey(secretBytes);
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
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