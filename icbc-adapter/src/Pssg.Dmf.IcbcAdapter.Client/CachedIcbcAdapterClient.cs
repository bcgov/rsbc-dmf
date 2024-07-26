using Microsoft.Extensions.Caching.Memory;
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

        public CachedIcbcAdapterClient(IMemoryCache cacheService, IcbcAdapterClient icbcAdapterClient, ILoggerFactory loggerFactory)
        {
            _cacheService = cacheService;
            _icbcAdapterClient = icbcAdapterClient;
            _logger = loggerFactory.CreateLogger<CachedIcbcAdapterClient>();
        }

        public async Task<DriverInfoReply> GetDriverInfoAsync(DriverInfoRequest request)
        {
            // TODO not completed but this is the basic idea
            // feature flag to return a simple response for ICBC in development environment, useful when Dynamics DL do not match ICBC DL
            //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //if (environment == Environments.Development && _configuration.GetValue<bool>("FEATURES_SIMPLE_ICBC"))
            //{
            //    result.FirstName = "John";
            //    result.LastName = "Smith";
            //    result.BirthDate = new DateTime(1980, 1, 1);
            //}

            DriverInfoReply reply = null;
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