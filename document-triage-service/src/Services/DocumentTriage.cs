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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Pssg.Rsbc.Dmf.DocumentTriage.Services
{

        // Default to require authorization
        [Authorize]
        public class DocumentTriageService : DocumentTriage.DocumentTriageBase
    {
            private readonly IConfiguration _configuration;
            private readonly ILogger<DocumentTriageService> _logger;

            public DocumentTriageService(ILogger<DocumentTriageService> logger, IConfiguration configuration)
            {
                _configuration = configuration;
                _logger = logger;
            }
    
            public override Task<TriageReply> Triage(TriageRequest request, ServerCallContext context)
            {
                var result = new TriageReply();

                _logger.LogInformation($"TRIAGE {request.Id}");

                bool cleanPass = true;
                foreach (var item in request.Flags)
                {
                    if (item.Result)
                    {
                        cleanPass = false;
                    }
                    _logger.LogInformation($"{item.Identifier} - {item.Question} - {item.Result}");
                }

                _logger.LogInformation($"CLEAN PASS is {cleanPass}");

            // update data in Dynamics here.

            result.ResultStatus = ResultStatus.Success;

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