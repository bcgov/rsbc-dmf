using Microsoft.Extensions.Caching.Memory;

namespace Rsbc.Dmf.DriverPortal.Api
{
    public interface ICacheService
    {
        bool TryGetValue<T>(string name, string key, out T? value);
        void Set<T>(string name, string key, T value);
    }

    public static class MemoryCacheServiceConfiguration
    {
        public static void AddMemoryCacheService(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }
    }

    public class MemoryCacheService : BaseCacheService, ICacheService, IDisposable
    {
        private readonly TimeSpan _expires = TimeSpan.FromHours(6);
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // name can be method name or service name
        public bool TryGetValue<T>(string name, string key, out T? value)
        {
            var hashKey = GetHashKey(name, key);
            return _memoryCache.TryGetValue(hashKey, out value);
        }

        // name can be method name or service name
        public void Set<T>(string name, string key, T value)
        {
            var hashKey = GetHashKey(name, key);
            _memoryCache.Set(hashKey, value, _expires);
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
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