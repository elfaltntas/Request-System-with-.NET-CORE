using Microsoft.Extensions.Caching.Memory;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class CacheManager : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();

            if (expiry.HasValue)
            {
                cacheEntryOptions.SetAbsoluteExpiration(expiry.Value);
            }

            _memoryCache.Set(key, value, cacheEntryOptions);
        }

        public void Remove(string RequestId)
        {
            _memoryCache.Remove(RequestId);
        }

        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out _);
        }
    }
}
