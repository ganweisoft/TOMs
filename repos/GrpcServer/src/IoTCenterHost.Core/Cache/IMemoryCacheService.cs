//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.Caching.Memory;
using System;

namespace IoTCenterHost.Core.Abstraction
{
    public interface IMemoryCacheService
    {
        T FindAndRemove<T>(string pattern);
        string Get(string key);
        T Get<T>(string key);
        bool IsSet(string key);
        void Remove(string key);
        void Set(string key, object obj);
        void Set(string key, object obj, DateTimeOffset escapedTime);

        void Set(string key, object obj, DateTimeOffset escapedTime,
            PostEvictionCallbackRegistration callbackRegistration);

    }
}
