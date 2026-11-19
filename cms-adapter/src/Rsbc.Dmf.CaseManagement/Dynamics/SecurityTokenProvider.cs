using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class SecurityTokenProvider : ISecurityTokenProvider
    {
        private readonly DynamicsOptions options;
        private readonly AdfsSecurityTokenProvider adfsSecurityTokenProvider;
        private readonly EntraIdSecurityTokenProvider entraIdSecurityTokenProvider;

        public SecurityTokenProvider(
            IOptions<DynamicsOptions> options,
            AdfsSecurityTokenProvider adfsSecurityTokenProvider,
            EntraIdSecurityTokenProvider entraIdSecurityTokenProvider
        )
        {
            this.options = options.Value;
            this.adfsSecurityTokenProvider = adfsSecurityTokenProvider;
            this.entraIdSecurityTokenProvider = entraIdSecurityTokenProvider;
        }

        public async Task<string> AcquireToken()
        {
            if (options.AuthenticationType == DynamicsAuthenticationType.Cloud)
            {
                return await entraIdSecurityTokenProvider.AcquireToken();
            }

            return await adfsSecurityTokenProvider.AcquireToken();
        }
    }
}
