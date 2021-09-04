using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Rsbc.Dmf.CaseManagement.Service;
using Serilog;

namespace Pssg.Rsbc.Dmf.DocumentTriage.Services
{

        // Default to require authorization
        [Authorize]
        public class DocumentTriageService : DocumentTriage.DocumentTriageBase
    {
            private readonly IConfiguration _configuration;
            private readonly ILogger<DocumentTriageService> _logger;
            private readonly CaseManager.CaseManagerClient _caseManagerClient;

            public DocumentTriageService(ILogger<DocumentTriageService> logger, IConfiguration configuration, CaseManager.CaseManagerClient caseManagerClient)
            {
                _caseManagerClient = caseManagerClient;
                _configuration = configuration;
                _logger = logger;
            }
    
            public override Task<TriageReply> Triage(TriageRequest request, ServerCallContext context)
            {
                var result = new TriageReply();

                _logger.LogInformation($"TRIAGE {request.Id}");

                // start by requesting the available flags.

                var availableFlags = _caseManagerClient.GetAllFlags(new GetAllFlagsRequest());

                bool cleanPass = true;

                List< global::Rsbc.Dmf.CaseManagement.Service.FlagItem > followUpFlags = new List<global::Rsbc.Dmf.CaseManagement.Service.FlagItem > ();

                foreach (var item in request.Flags)
                {
                    if (item.Result)
                    {
                        var followUpFlag = new global::Rsbc.Dmf.CaseManagement.Service.FlagItem()
                        {
                            Question = item.Question,
                            Identifier = item.Identifier,
                            Result = item.Result
                        };
                        var matchFlag = availableFlags.Flags.Where(x => x.Identifier == item.Identifier).FirstOrDefault();
                        
                        if (matchFlag != null)
                        {
                            followUpFlag.FlagType = matchFlag.FlagType;
                          // S3DMFT-926 - set clean pass based on follow up or review flags.
                            if (matchFlag.FlagType == global::Rsbc.Dmf.CaseManagement.Service.FlagItem.Types
                                .FlagTypeOptions.FollowUp || matchFlag.FlagType == global::Rsbc.Dmf.CaseManagement.Service.FlagItem.Types.FlagTypeOptions.Review)
                            {
                                cleanPass = false;
                            }
                        }
                        else
                        {
                            followUpFlag.FlagType = global::Rsbc.Dmf.CaseManagement.Service.FlagItem.Types
                                .FlagTypeOptions.Unknown;
                        }
                    }
                    _logger.LogInformation($"{item.Identifier} - {item.Question} - {item.Result}");
                }

                _logger.LogInformation($"CLEAN PASS is {cleanPass}");

            // update data in Dynamics here.

            UpdateCaseRequest updateCaseRequest = new UpdateCaseRequest()
            {
                CaseId = request.Id,
                IsCleanPass = cleanPass,
                DataFileKey = request.DataFileKey,
                PdfFileKey = request.PdfFileKey
            };

            foreach(var item in followUpFlags)
            {
                updateCaseRequest.Flags.Add(item);
            }

            var caseResult = _caseManagerClient.UpdateCase(updateCaseRequest);

            _logger.LogInformation($"Case Update Result is {caseResult.ResultStatus}");

            result.ResultStatus = ResultStatus.Success;
            result.ErrorDetail = "";

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