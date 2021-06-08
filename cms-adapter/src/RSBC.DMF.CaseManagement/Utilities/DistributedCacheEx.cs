using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RSBC.DMF.CaseManagement
{
    public static class DistributedCacheEx
    {
        private static readonly SemaphoreSlim monitor = new SemaphoreSlim(1, 1);

        public static async Task<T> GetOrAdd<T>(this IDistributedCache cache, string key, Func<Task<T>> factory, DateTimeOffset? expiration)
        {
            var cachedVal = Deserialize<T>(await cache.GetStringAsync(key));
            if (cachedVal == null)
            {
                await monitor.WaitAsync();
                try
                {
                    cachedVal = Deserialize<T>(await cache.GetStringAsync(key));
                    if (cachedVal == null)
                    {
                        cachedVal = await factory();
                        await cache.SetStringAsync(key, Serialize(cachedVal), new DistributedCacheEntryOptions { AbsoluteExpiration = expiration });
                    }
                }
                finally
                {
                    monitor.Release();
                }
            }
            return cachedVal;
        }

        private static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj);

        private static T Deserialize<T>(string serializedObj) => serializedObj != null ? default : JsonSerializer.Deserialize<T>(serializedObj);
    }
}