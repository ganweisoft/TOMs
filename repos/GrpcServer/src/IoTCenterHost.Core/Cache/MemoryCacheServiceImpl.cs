//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;

namespace IoTCenterHost.Core.Abstraction
{
    public class MemoryCacheServiceImpl : IMemoryCacheService
    {
        private readonly IMemoryCache MemoryCache;
        public MemoryCacheServiceImpl()
        {
            MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }
        public T FindAndRemove<T>(string pattern)
        {
            var obj = default(T);
            MemoryCache.TryGetValue<T>(pattern, out obj);
            MemoryCache.Remove(pattern);
            return obj;
        }

        public string Get(string key)
        {
            MemoryCache.TryGetValue(key, out string result);
            return result;
        }

        public T Get<T>(string key)
        {
            var obj = default(T);
            MemoryCache.TryGetValue<T>(key, out obj);
            return obj;
        }

        public bool IsSet(string key)
        {
            return MemoryCache.Get(key) != null;
        }

        public void Remove(string key)
        {
            MemoryCache.Remove(key);
        }

        public void Set(string key, object obj)
        {
            MemoryCache.Set(key, obj);
        }

        public void Set(string key, object obj, DateTimeOffset escapedTime)
        {
            MemoryCache.Set(key, obj, escapedTime);
        }

        public void Set(string key, object obj, DateTimeOffset escapedTime, PostEvictionCallbackRegistration callbackRegistration)
        {
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource((escapedTime - DateTimeOffset.Now).Add(TimeSpan.FromSeconds(0.01))).Token);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetAbsoluteExpiration(escapedTime)
                .AddExpirationToken(expirationToken)
                .RegisterPostEvictionCallback(callbackRegistration.EvictionCallback, state: callbackRegistration.State);

            MemoryCache.Set(key, obj, cacheEntryOptions);
        }
    }
}
