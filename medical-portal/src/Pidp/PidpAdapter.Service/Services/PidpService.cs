using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using PidpAdapter.Endorsement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PidpAdapter.Services
{
    public class PidpService : PidpManager.PidpManagerBase
    {
        private readonly ILogger<PidpService> _logger;
        private readonly IEndorsement _endorsement;
        private readonly IConfiguration _configuration;

        public PidpService(ILogger<PidpService> logger, IEndorsement endorsement, IConfiguration configuration)
        {
            _logger = logger;
            _endorsement = endorsement;
            _configuration = configuration;
        }

        // TODO check if this can be moved to SharedUtils
        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            var result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            var configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (configuredSecret != null && !string.IsNullOrEmpty(request?.Secret) && configuredSecret.Equals(request.Secret))
            {
                byte[] key = Encoding.UTF8.GetBytes(_configuration["JWT_TOKEN_KEY"]);
                Array.Resize(ref key, 32);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

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

        public override async Task<GetEndorsementsReply> GetEndorsements(GetEndorsementsRequest request, ServerCallContext context)
        {
            var response = new GetEndorsementsReply();

            try
            {
                // real data from Pidp with no results
                request.UserId = "kkpqtjseoyaygbqxmjq7kltol7wffrn6";
                var endorsements = await _endorsement.GetEndorsement(request.UserId);
                // TODO check fail and status code

                // fake data to test with results, until above is working
                var license = new LicenseDto
                {
                    IdentifierType = "1",
                    StatusCode = "2",
                    StatusReasonCode = "3"
                };
                var endorsement = new EndorsementDto
                {
                    UserId = "2p3qysl7dn6kvboslzc2k7uimevuxusk",
                };
                endorsement.Licenses.AddRange(new List<LicenseDto> { license });
                var endorsementList = new List<EndorsementDto>();
                endorsementList.Add(endorsement);
                response.Endorsements.AddRange(endorsementList);

                // TODO
                //response.Endorsements.AddRange(endorsements.Select(endorsement => new EndorsementDto
                //{
                //    UserId = endorsement.Hpdid,
                //    Licences = endorsement.Licences.Select(licence => new LicenseDto
                //    {
                //        IdentifierType = licence.IdentifierType,
                //        StatusCode = licence.StatusCode,
                //        StatusReasonCode = licence.StatusReasonCode
                //    }).ToList()
                //}));
                //response.Endorsements.AddRange(_mapper.Map<IEnumerable<EndorsementDto>>(endorsements));
                response.ResultStatus = ResultStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting endorsements");
                response.ResultStatus = ResultStatus.Fail;
                response.ErrorDetail = "Error getting endorsements";
            }

            return response;
        }
    }
}
