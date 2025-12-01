// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;

namespace IoTCenter.Utilities
{
    public interface IMemoryCacheService
    {
        T FindAndRemove<T>(string pattern);
        string Get(string key);
        T Get<T>(string key);
        bool TryGetValue(string key, out object value);
        bool IsSet(string key);
        void Remove(string key);
        void Set(string key, object obj);
        void Set(string key, object obj, DateTimeOffset escapedTime);

        IEnumerable<string> Get(string key, string[] hashField);

        void Set(string key, IEnumerable<KeyValuePair<string, string>> hashValues, TimeSpan timeSpan);
    }
}
