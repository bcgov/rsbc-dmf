using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Rsbc.Dmf.IcbcAdapter.IcbcAdapter;

namespace Rsbc.Dmf.IcbcAdapter.Client
{
    public interface ICachedIcbcAdapterClient
    {
        Task<DriverInfoReply> GetDriverInfoAsync(DriverInfoRequest request);
    }

    public class CachedIcbcAdapterClient : BaseCacheService, ICachedIcbcAdapterClient, IDisposable
    {
        private readonly IMemoryCache _cacheService;
        private readonly IcbcAdapterClient _icbcAdapterClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public CachedIcbcAdapterClient(IMemoryCache cacheService, IcbcAdapterClient icbcAdapterClient, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _cacheService = cacheService;
            _icbcAdapterClient = icbcAdapterClient;
            _logger = loggerFactory.CreateLogger<CachedIcbcAdapterClient>();
            _configuration = configuration;
        }

        public async Task<DriverInfoReply> GetDriverInfoAsync(DriverInfoRequest request)
        {
            DriverInfoReply reply = null;

            // feature flag to return a simple response for ICBC in development environment, useful when Dynamics DL do not match ICBC DL
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Development && _configuration["FEATURES_SIMPLE_ICBC"] == "true")
            {
                reply = new DriverInfoReply();
                reply.GivenName = "John";
                reply.Surname = "Smith";
                reply.BirthDate = new DateTime(1980, 1, 1).ToString();
                reply.AddressLine1 = "123 Main St";
                reply.City = "Victoria";
                reply.Country = "Canada";
                reply.Postal = "V8V 3V3";
                reply.Province = "BC";
                reply.Sex = "M";
                reply.ResultStatus = ResultStatus.Success;
                return reply;
            }

            try
            {
                var key = GetHashKey(nameof(IcbcAdapterClient.GetDriverInfo), request.DriverLicence);
                if (!_cacheService.TryGetValue(key, out reply))
                {
                    reply = await _icbcAdapterClient.GetDriverInfoAsync(request);
                    _cacheService.Set(key, reply);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(IcbcAdapterClient.GetDriverInfoAsync)} {nameof(IMemoryCache)} failed.");
                reply = await _icbcAdapterClient.GetDriverInfoAsync(request);
            }

            return reply;
        }

        public void Dispose()
        {
            _cacheService.Dispose();
        }
    }

    public class BaseCacheService
    {
        protected string GetHashKey(string name, string key)
        {
            return $"{name.GetHashCode()}/{key}";
        }
    }
}