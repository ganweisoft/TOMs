// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.DependencyInjection;

namespace System;

public static class ServiceLocator
{
    private static IServiceProvider _current;
    public static IServiceProvider Current
    {
        get
        {
            if (_current == null)
                return null;
            var provider = _current;

            var scope = provider?
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            if (scope == null)
            {
                throw new ArgumentException($"create scope occur exception：{nameof(scope)}");
            }

            return scope.ServiceProvider;
        }
    }

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _current = serviceProvider;
    }
}
