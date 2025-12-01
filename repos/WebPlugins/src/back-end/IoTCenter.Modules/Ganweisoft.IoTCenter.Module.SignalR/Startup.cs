// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ganweisoft.IoTCenter.Module.SignalR
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<MonitorHub>();

            services.AddSingleton<SignalrProducer>();

            services.AddSignalR();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<SignalrProducer>();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MonitorHub>("/Monitor");
            });
        }
    }
}