// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Internal.Extensions;
using IoTCenterCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ganweisoft.IoTCenter.Module.EquipLink
{
    public class Startup : StartupBase
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddIoTDbContext<EquipLinkDbContext>(Configuration);

            services.AddScoped<IEquipLinkService, EquipLinkServiceImpl>();

            services.AddScoped<IConditionLinkageService, ConditionLinkageServiceImpl>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapDefaultControllerRoute();
        }
    }
}