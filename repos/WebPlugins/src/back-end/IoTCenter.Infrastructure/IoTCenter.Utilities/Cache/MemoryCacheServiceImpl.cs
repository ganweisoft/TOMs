// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenter.Utilities
{
    public class MemoryCacheServiceImpl : IMemoryCacheService
    {
        private readonly MemoryCacheServiceProvider memoryCacheServiceProvider;
        public MemoryCacheServiceImpl(MemoryCacheServiceProvider memoryCacheServiceProvider)
        {
            this.memoryCacheServiceProvider = memoryCacheServiceProvider;
        }
        public T FindAndRemove<T>(string pattern)
        {
            return memoryCacheServiceProvider.FindAndRemove<T>(pattern);
        }

        public string Get(string key)
        {
            return memoryCacheServiceProvider.Get(key);
        }

        public T Get<T>(string key)
        {
            return memoryCacheServiceProvider.Get<T>(key);
        }

        public IEnumerable<string> Get(string key, string[] hashField)
        {
            return memoryCacheServiceProvider.Get(key, hashField);
        }

        public bool IsSet(string key)
        {
            return memoryCacheServiceProvider.IsSet(key);
        }

        public void Remove(string key)
        {
            memoryCacheServiceProvider.Remove(key);
        }

        public void Set(string key, object obj)
        {
            memoryCacheServiceProvider.Set(key, obj);
        }

        public void Set(string key, object obj, DateTimeOffset escapedTime)
        {
            memoryCacheServiceProvider.Set(key, obj, escapedTime);
        }

        public void Set(string key, IEnumerable<KeyValuePair<string, string>> hashValues, TimeSpan timeSpan)
        {
            memoryCacheServiceProvider.Set(key, hashValues, System.DateTime.Now.Add(timeSpan));
        }

        public bool TryGetValue(string key, out object value)
        {
            return memoryCacheServiceProvider.TryGetValue(key, out value);
        }
    }
}
