//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices;
using IoTCenterHost.Core;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using IoTCenterHost.Dapr;
using IoTCenterHost.Dapr.Controllers;

namespace GwWebHost
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly bool _isEnableGrpc = false;
        private readonly string language;
        public static bool HostStart = false;
        private static bool _gwDataCenterStarted;

        private IServiceCollection serviceDescriptors = null;
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            var directoryInfo = new System.IO.DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;
            string configurationXml = Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");
            string enableGrpcFlag = FileExtension.ReadXml(configurationXml, "HostServer", "EnableGrpc"); //GWDataCenter.DataCenter.GetPropertyFromPropertyService("HostServer", "EnableGrpc", "");

            language = FileExtension.ReadXml(configurationXml, "", "", "CoreProperties.UILanguage");
            if (string.IsNullOrEmpty(language))
                language = "zh-CN";

            if (string.IsNullOrEmpty(enableGrpcFlag))
            {
                if (string.Equals(language, "en-US"))
                {
                    Serilog.Log.Logger.Error("Fail To Read the Configuration Option For ��HostServer:EnableGrpc�� In AlarmCenterProperties.xml,Use The Default Config In Appsetting.json ");
                }
                else
                    Serilog.Log.Logger.Error("在AlarmCenterProperties.xml文件中未读取到配置【HostServer:EnableGrpc】项，已使用appsetting.json中的默认配置\r\n");
                enableGrpcFlag = configuration["HostServer:EnableGrpc"];
            }
            _isEnableGrpc = string.Equals(enableGrpcFlag, "true", StringComparison.InvariantCultureIgnoreCase);

            Environment = hostEnvironment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(DataReportController).Assembly;
            var mvcBuilder = services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                });

            mvcBuilder.AddHostDapr();
            mvcBuilder.AddApplicationPart(assembly);

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
            services.AddLocalization(options => options.ResourcesPath = "Resources");


            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            IoTCenterHostGrpcExtension.InjectGrpcServices(services, _configuration, Environment);
            serviceDescriptors = services.InjectDBContext(_configuration);
            InjectStartGatewayExtension.InjectStartGateway(serviceDescriptors, language);
            InjectStartGatewayExtension.GWDataCenterStartedEvent += (_, _) =>
            {
                _gwDataCenterStarted = true;
            };


#if DEBUG
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));
#endif

            var servicesProvider = services.BuildServiceProvider();
            ServiceLocator.SetRootServiceProvider(servicesProvider);
            if (_isEnableGrpc)
            {
                services.RegistAuthentication(_configuration);
                services.AddAuthorization();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                services.AddGrpcHealthChecks();
            }

            services.AddHealthChecks()
            .AddCheck<AppHealthCheck>("AppHealthCheck");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Add dapr sub
            app.UseDapr();

            app.UseGrpcWeb();

            app.UseAuthentication();
            app.UseAuthorization();

            IList<CultureInfo> supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en-US"),
                            new CultureInfo("zh-CN"),
                        };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(language),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

#if DEBUG
            app.UseCors();
#endif

            app.UseEndpoints(endpoints =>
            {
                // Add dapr sub
                endpoints.MapDapr();
                endpoints.MapControllers();

                if (_isEnableGrpc)
                {
                    IoTCenterHostGrpcExtension.MapGrpcServices(endpoints);
                }

                endpoints.MapGet("/", async context =>
                {
                    if (string.Equals(language, "en-US"))
                    {
                        await context.Response.WriteAsync("Welcome Use IotCenter.\r\n");
                    }
                    else
                        await context.Response.WriteAsync("欢迎使用IotCenter.\r\n");
                });

                endpoints.MapGet("/test", async context =>
                {
                    await context.Response.WriteAsync("welcome\r\n");
                });

                /*定义AppHealthCheck健康检查路径*/
                endpoints.MapHealthChecks("/AppHealthCheck", new HealthCheckOptions()
                {
                    Predicate = s => s.Name.Equals("AppHealthCheck"),
                });
                endpoints.MapGrpcHealthChecksService();
            });

            string logStr = "";
            if (string.Equals(language, "en-US"))
            {
                logStr = "IoTCenter Gateway Server Was Initiated.";
            }
            else
                logStr = "IotCenter网关服务端已启动.";
            Serilog.Log.Logger.Information(logStr);

            Console.Out.WriteLine(logStr);

            var memCache = serviceProvider.GetService<IMemoryCacheService>();
            memCache?.Set(GreeterService.HostStartTimeMemKey, DateTime.Now);

            HostStart = true;
        }
    }
    public class AppHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            ThreadPool.GetAvailableThreads(out int threads, out var portThreads);
            var msg = $"thread count: {ThreadPool.ThreadCount}, worker count: {threads}, port count: {portThreads}\r\n";

            return Task.FromResult(HealthCheckResult.Healthy(msg));
        }
    }
}
