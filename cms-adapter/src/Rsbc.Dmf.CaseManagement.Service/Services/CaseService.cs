using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Rsbc.Dmf.CaseManagement.Service
{
    //[Authorize]
    public class CaseService : CaseManager.CaseManagerBase
    {
        private readonly ILogger<CaseService> _logger;
        private readonly ICaseManager _caseManager;
        private readonly IConfiguration _configuration;

        public CaseService(ILogger<CaseService> logger, ICaseManager caseManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _caseManager = caseManager;
        }

        public async override Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            var searchResult = await _caseManager.CaseSearch(new CaseSearchRequest { ByCaseId = request.CaseId });

            var reply = new SearchReply();
            reply.Items.Add(searchResult.Items.Select(c => new Case
            {
                CaseId = c.Id,
                CreatedBy = c.CreatedBy,
                CreatedOn = Timestamp.FromDateTime(c.CreatedOn)
            }));

            return reply;
        }

        public async override Task<UpdateCaseReply> UpdateCase(UpdateCaseRequest request, ServerCallContext context)
        {
            var reply = new UpdateCaseReply();

            _logger.LogInformation($"UPDATE CASE - {request.CaseId}, clean pass is {request.IsCleanPass}, files - {request.DataFileKey} {request.PdfFileKey}");

            // convert the flags to a list of strings.

            List<Flag> flags = new List<Flag>();

            foreach (var item in request.Flags)
            {
                Flag newFlag = new Flag()
                {
                    Description = item.Question,
                    Id = item.Identifier
                };
                flags.Add(newFlag);
                _logger.LogInformation($"Added flag {item.Question} to flags for set case flags.");
            }

            // set the flags.

            var x = await _caseManager.SetCaseFlags(request.CaseId, flags, _logger);
            _logger.LogInformation($"Set Flags result is {x.Success}.");
            reply.ResultStatus = ResultStatus.Success;
            return reply;
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