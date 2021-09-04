using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rsbc.Dmf.CaseManagement.Service.FlagItem.Types;

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
            var cases = (await _caseManager.CaseSearch(new CaseSearchRequest
            {
                CaseId = request.CaseId,
                ClinicId = request.ClinicId,
                DriverLicenseNumber = request.DriverLicenseNumber
            })).Items.Cast<Rsbc.Dmf.CaseManagement.DmerCase>();

            var reply = new SearchReply();
            reply.Items.Add(cases.Select(c =>
            {
                var newCase = new DmerCase
                {
                    CaseId = c.Id,
                    CreatedBy = c.CreatedBy ?? string.Empty,
                    CreatedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    ModifiedBy = c.CreatedBy ?? string.Empty,
                    ModifiedOn = Timestamp.FromDateTime(c.CreatedOn.ToUniversalTime()),
                    DriverLicenseNumber = c.DriverLicenseNumber ?? string.Empty,
                    DriverName = c.DriverName ?? string.Empty,
                };
                newCase.Flags.Add(c.Flags.Select(f => new FlagItem { Identifier = f.Id, Question = f.Description ?? "Unknown", FlagType = ConvertFlagType(f.FlagType)}));
                return newCase;
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

            var x = await _caseManager.SetCaseFlags(request.CaseId, request.IsCleanPass, flags);
            _logger.LogInformation($"Set Flags result is {x.Success}.");

            // update files.

            _logger.LogInformation($"Add file - {request.CaseId}, file is {request.IsCleanPass}, files - {request.DataFileKey} {request.PdfFileKey}");

            if (!string.IsNullOrEmpty(request.PdfFileKey))
            {
                await _caseManager.AddDocumentUrlToCaseIfNotExist(request.CaseId, request.PdfFileKey);
            }
            if (!string.IsNullOrEmpty(request.DataFileKey))
            {
                await _caseManager.AddDocumentUrlToCaseIfNotExist(request.CaseId, request.DataFileKey);
            }
            

            reply.ResultStatus = ResultStatus.Success;
            return reply;
        }

        FlagTypeOptions ConvertFlagType(FlagTypeOptionSet? value)
        {
            FlagTypeOptions result = FlagTypeOptions.Unknown;
            switch (value)
            {
                case FlagTypeOptionSet.FollowUp:
                    result = FlagTypeOptions.FollowUp;
                    break;
                case FlagTypeOptionSet.Message:
                    result = FlagTypeOptions.Message;
                    break;
                case FlagTypeOptionSet.Review:
                    result = FlagTypeOptions.Review;
                    break;
                case FlagTypeOptionSet.Submittal:
                    result = FlagTypeOptions.Submittal;
                    break;
            }

            return result;
        }

        public async override Task<GetAllFlagsReply> GetAllFlags(GetAllFlagsRequest request, ServerCallContext context)
        {
            var reply = new GetAllFlagsReply();
            var flags = await _caseManager.GetAllFlags();
            foreach (var flag in flags)
            {
                FlagItem newFlag = new FlagItem()
                {
                    Identifier = flag.Id,
                    Question = flag.Description ?? "",
                    FlagType = ConvertFlagType (flag.FlagType)
                };
                reply.Flags.Add(newFlag);
            }

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