// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Internal.Extensions;
using IoTCenter.Data;
using IoTCenter.Utilities;
using IoTCenterCore.ExcelHelper;
using IoTCenterCore.RsaEncrypt;
using IoTCenterCore.SlideVerificationCode;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Proxy;
using IoTCenterHost.Proxy.Core;
using IoTCenterHost.Proxy.ServiceImpl;
using IoTCenterWebApi.BaseCore;
using IoTCenterWebApi.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using System;
using System.IO.Compression;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace IoTCenterWebApi
{
    public static class IoTCenterCollectionExtensions
    {
        internal static IServiceCollection AddInternalService(this IServiceCollection services)
        {
            var mvcBuilders = services.AddControllers(options =>
            {
                options.Filters.Add(typeof(WebGlobalExecptionFilter));
                options.Filters.Add(typeof(CustomFilterAttribute));
                options.Filters.Add(typeof(PermissionHandlerAttribute));
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

                options.JsonSerializerOptions.Converters.Add(new StringConverter());
                options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterForNullableDateTime());
                options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterForNullableInt());
                options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterForDateTime());
            })
            .AddSessionStateTempDataProvider();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
            });

            services.AddHttpContextAccessor();

            services.AddResponseCaching();

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = false);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var modelState = context.ModelState;
                    var result = string.Join(',', modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x => x.ErrorMessage)));
                    return new JsonResult(OperateResult.Failed($"参数输入失败:{result}"));
                };
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });

            return services;
        }

        public static IServiceCollection AddIoTService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<WebApiConfigOptions>(configuration.GetSection("WebApi"));

            services.AddSingleton<IRSAAlgorithm, RSAAlgorithm>();

            services.AddSingleton<CaptchaHelper>();

            services.AddScoped<IExportManager, ExportManager>();

            services.AddScoped<IImportManager, ImportManager>();

            services.AddLogging();

            services.AddScoped<ILoggingService, LoggingService>();
                services.AddSingleton<Serilogger>();

            services.AddIoTDbContext<GWDbContext>(configuration);

            services.AddSingleton<PermissionCacheService>();

            services.AddJwtConfiguration(configuration);

            services.AddScoped<IServiceManageService, ServiceManageServiceImpl>();

            services.AddScoped<CookieAuthenticationEventsExtensions>();

            services.AddMemoryCache();

            services.AddDefaultDynamicCache();

            services.AddSingleton<MemoryCacheServiceProvider>();
            services.AddSingleton<IMemoryCacheService, MemoryCacheServiceImpl>();

            services.AddScoped<Session>();

            services.AddScoped<EquipBaseImpl>();

            return services;
        }
    }
}
