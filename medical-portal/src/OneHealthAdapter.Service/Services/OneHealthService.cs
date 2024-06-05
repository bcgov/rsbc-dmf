using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OneHealthAdapter.Endorsement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace OneHealthAdapter.Services
{
    public class OneHealthService : OneHealthManager.OneHealthManagerBase
    {
        private readonly ILogger<OneHealthService> _logger;
        private readonly IEndorsement _endorsement;
        private readonly IConfiguration _configuration;

        public OneHealthService(ILogger<OneHealthService> logger, IEndorsement endorsement, IConfiguration configuration)
        {
            _logger = logger;
            _endorsement = endorsement;
            _configuration = configuration;
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

            var configuredSecret = _configuration["Jwt:Secret"];
            if (configuredSecret != null && !string.IsNullOrEmpty(request?.Secret) && configuredSecret.Equals(request.Secret))
            {
                byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
                Array.Resize(ref key, 32);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
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
                var endorsements = await _endorsement.GetEndorsement(request.UserId);

                // fake data
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
