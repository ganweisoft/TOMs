// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.Modules;
using IoTCenterWebApi.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ganweisoft.IoTCenter.Module.EquipList
{
    public class Startup : StartupBase
    {
        public IIoTConfiguration _configuration { get; set; }

        public Startup(IIoTConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CacheJobOptions>(
                _configuration.GetSection($"Ganweisoft.IoTCenter.Module.EquipList:{CacheJobOptions.CacheOption}"));

            services.AddScoped<IBAService, BAServicelmpl>();
            services.AddScoped<IEGroupService, EGroupServiceImpl>();
            services.AddScoped<IEquipListService, EquipListServiceImpl>();

            services.AddSingleton<YcYxEquipStatusChangeNotify>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapDefaultControllerRoute();

            routes.MapHub<DownFileNotifyHub>("/downFileNotify");
            routes.MapHub<EGroupHub>("/eGroupNotify");
            routes.MapHub<EquipStatusMonitorHub>("/equipStatusMonitor");
            routes.MapHub<PointStatusMonitorHub>("/pointStatusMonitor");

            serviceProvider.GetRequiredService<YcYxEquipStatusChangeNotify>();
        }
    }
}