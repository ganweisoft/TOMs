// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenter.Utilities
{
    public class MemoryCacheServiceProvider
    {
        private readonly IMemoryCache MemoryCache;
        public MemoryCacheServiceProvider(IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
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

        public IEnumerable<string> Get(string key, string[] hashField)
        {
            var result = MemoryCache.Get<IEnumerable<KeyValuePair<string, string>>>(key);
            return (hashField != null && hashField.Any())? result.Where(u => hashField.Contains(u.Key)).Select(u => u.Value) : result.Select(u=>u.Value);
        }

        public bool IsSet(string key)
        {
            return MemoryCache.Get(key) != null;
        }

        public void Remove(string key)
        {
            if (MemoryCache.TryGetValue(key, out _))
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

        public void Set(string key, IEnumerable<KeyValuePair<string, string>> hashValues, TimeSpan timeSpan)
        {
            MemoryCache.Set(key, hashValues, System.DateTime.Now.Add(timeSpan));
        }

        public bool TryGetValue(string key, out object value)
        {
            value = Get<object>(key);
            return value != null;
        }
    }
}
