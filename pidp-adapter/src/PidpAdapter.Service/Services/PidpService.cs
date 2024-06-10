using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PidpService(ILogger<PidpService> logger, IEndorsement endorsement, IMapper mapper, IConfiguration configuration)
        {
            _logger = logger;
            _endorsement = endorsement;
            _mapper = mapper;
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
                var endorsements = await _endorsement.GetEndorsement(request.UserId);
                // TODO check fail and status code
                var mappedEndorsements = _mapper.Map<IEnumerable<EndorsementDto>>(endorsements);
                response.Endorsements.AddRange(mappedEndorsements);
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
