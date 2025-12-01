using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using IoTCenterCore.Modules;

namespace Microsoft.Extensions.DependencyInjection
{
    public class IoTCenterCoreBuilder
    {
        private Dictionary<int, StartupActions> _actions { get; } = new Dictionary<int, StartupActions>();

        public IoTCenterCoreBuilder(IServiceCollection services)
        {
            ApplicationServices = services;
        }

        public IServiceCollection ApplicationServices { get; }

        public IoTCenterCoreBuilder RegisterStartup<T>() where T : class, IStartup
        {
            ApplicationServices.AddTransient<IStartup, T>();
            return this;
        }

        public IoTCenterCoreBuilder ConfigureServices(Action<IServiceCollection, IServiceProvider> configure, int order = 0)
        {
            if (!_actions.TryGetValue(order, out var actions))
            {
                actions = _actions[order] = new StartupActions(order);

                ApplicationServices.AddTransient<IStartup>(sp => new StartupActionsStartup(
                    sp.GetRequiredService<IServiceProvider>(), actions, order));
            }

            actions.ConfigureServicesActions.Add(configure);

            return this;
        }

        public IoTCenterCoreBuilder ConfigureServices(Action<IServiceCollection> configure, int order = 0)
        {
            return ConfigureServices((s, sp) => configure(s), order);
        }

        public IoTCenterCoreBuilder Configure(Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider> configure, int order = 0)
        {
            if (!_actions.TryGetValue(order, out var actions))
            {
                actions = _actions[order] = new StartupActions(order);

                ApplicationServices.AddTransient<IStartup>(sp => new StartupActionsStartup(
                    sp.GetRequiredService<IServiceProvider>(), actions, order));
            }

            actions.ConfigureActions.Add(configure);

            return this;
        }

        public IoTCenterCoreBuilder Configure(Action<IApplicationBuilder, IEndpointRouteBuilder> configure, int order = 0)
        {
            return Configure((app, routes, sp) => configure(app, routes), order);
        }

        public IoTCenterCoreBuilder Configure(Action<IApplicationBuilder> configure, int order = 0)
        {
            return Configure((app, routes, sp) => configure(app), order);
        }
    }
}
