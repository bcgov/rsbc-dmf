using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pssg.Interfaces
{
    public class S3HealthCheck : IHealthCheck
    // reference https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2
    {
        private readonly IConfiguration _configuration;

        public S3HealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
        {
            S3 sharepoint = new S3(_configuration);
            // Try and get the Account document library
            bool healthCheckResultHealthy;
            try
            {
                var result = sharepoint.GetDocumentLibrary("Account").GetAwaiter().GetResult();

                healthCheckResultHealthy = (result != null);
            }
            catch (Exception)
            {
                healthCheckResultHealthy = false;
            }

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Sharepoint is healthy."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Sharepoint is unhealthy."));
        }
    }
}
