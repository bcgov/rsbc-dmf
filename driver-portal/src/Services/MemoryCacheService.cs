using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Rsbc.Dmf.DriverPortal.Api.Services
{
    public interface ICacheService
    {
        bool TryGetValue<T>(string name, string key, out T? value);
        void Set<T>(string name, string key, T value);
    }

    //public class DistributedCacheService : BaseCacheService, ICacheService, IDisposable
    //{
    //    private readonly IDistributedCache _distributedCache;

    //    public DistributedCacheService(IDistributedCache distributedCache)
    //    {
    //        _distributedCache = distributedCache;
    //    }

    //    public bool TryGetValue<T>(string name, string key, out T? value)
    //    {
    //        var hashKey = GetHashKey(name, key);
    //        return _distributedCache.TryGetValue(hashKey, out value);
    //    }

    //    public void Set<T>(string name, string key, T value)
    //    {
    //        var hashKey = GetHashKey(name, key);
    //        _distributedCache.Set(hashKey, value, _expires);
    //    }

    //    public void Dispose()
    //    {
    //        _distributedCache.Dispose();
    //    }
    //}

    public class MemoryCacheService : BaseCacheService, ICacheService, IDisposable
    {
        // TODO should be overridable on initialization
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