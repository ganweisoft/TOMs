//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using AutoMapper;
using IoTCenterHost.AppServices.Application;
using IoTCenterHost.AppServices.AppServices;
using IoTCenterHost.AppServices.Domain.PO;
using IoTCenterHost.AppServices.GrpcConstracts;
using IoTCenterHost.AppServices.GrpcConstracts.IotHostService;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.AppServices.Interfaces.Token;
using IoTCenterHost.AppServices.StartUp;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.Startup;
using IoTCenterHost.GrpcConstract.GrpcConstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace IoTCenterHost.AppServices
{
    public static class IoTCenterHostGrpcExtension
    {
        public static void InjectGrpcServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment Environment)
        {
            services
        .InjectAppService()
        .InjectRepositoryService()
        .InjectSingletonService()
        .AddScoped<ITokenService, JwtTokenServiceImpl>()
        .AddScoped<IotCenterService, IotCenterServiceImpl>()
        .AddSingleton<IHostConfigurationAppService, HostConfigurationAppServiceImpl>()
        .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
           .AddGrpc(options =>
           {
               options.Interceptors.Add<ServerCallbackInterceptor>();
           });
            AutoMapperExtension.SetMapper(new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            }));


        }
        public static void MapGrpcServices(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb();
            endpoints.MapGrpcService<IotHostService>().EnableGrpcWeb();
            endpoints.MapGrpcService<IotQueryService>().EnableGrpcWeb();
            endpoints.MapGrpcService<IotCallbackService>().EnableGrpcWeb();
            endpoints.MapGrpcService<IotSubgatewayService>().EnableGrpcWeb();
        }


        private static IServiceProvider CreateTempProvider()
        {
            IServiceProvider provider;
            var directoryInfo = new System.IO.DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;
            string configurationXml = Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");
            string enableGrpcFlag = IoTCenterHost.Core.Extension.FileExtension.ReadXml(configurationXml, "HostServer", "EnableGrpc"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("HostServer", "EnableGrpc", "");

            string language = IoTCenterHost.Core.Extension.FileExtension.ReadXml(configurationXml, "", "", "CoreProperties.UILanguage");
            IServiceCollection services = new ServiceCollection();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var supportedCultures = new[]
{
                            new CultureInfo("zh-CN"),
                            new CultureInfo("en-US"),
                        };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(language);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            var config = new ConfigurationBuilder()
                 .SetBasePath(Path.GetDirectoryName(typeof(IoTCenterHostGrpcExtension).Assembly.Location))
                 .AddJsonFile("appsettings.json", optional: true)
                .Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddScoped<ILoggerFactory, LoggerFactory>();
            services.AddDbContext<GanweiDbContext>();
            provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
