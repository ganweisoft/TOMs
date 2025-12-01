//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IoTCenterHost.Core
{
    public static class ServiceLocator
    {
        public static IServiceProvider Root { get; private set; }
        public static IServiceProvider Current { get; private set; }

        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider ?? Current;

            var scope = provider?
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            if (scope == null)
            {
                throw new Exception($"create scope occur exception：{nameof(scope)}");
            }

            Current = scope.ServiceProvider;
        }



        public static void SetRootServiceProvider(IServiceProvider serviceProvider)
        {
            Root = serviceProvider;
        }
    }
}
