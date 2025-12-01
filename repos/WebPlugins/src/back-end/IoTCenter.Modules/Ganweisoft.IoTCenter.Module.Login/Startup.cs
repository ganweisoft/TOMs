// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ganweisoft.IoTCenter.Module.Login
{
    public class Startup : StartupBase
    {
        public override int Order => -100;
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthServiceImpl>();

            services.AddScoped<IUserManageService, UserManageServiceImpl>();

        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapDefaultControllerRoute();

            builder.UseAuthentication();

            builder.UseAuthorization();
        }
    }
}