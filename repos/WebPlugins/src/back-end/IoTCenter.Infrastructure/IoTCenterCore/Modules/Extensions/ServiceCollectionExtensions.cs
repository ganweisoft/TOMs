using AutoMapper;
using AutoMapper.Configuration;
using IoTCenterCore.Abstractions;
using IoTCenterCore.AutoMapper;
using IoTCenterCore.Environment.Extensions;
using IoTCenterCore.Environment.Shell;
using IoTCenterCore.Environment.Shell.Builders;
using IoTCenterCore.Environment.Shell.Configuration;
using IoTCenterCore.Environment.Shell.Descriptor.Models;
using IoTCenterCore.Localization;
using IoTCenterCore.Modules;
using IoTCenterCore.Modules.FileProviders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IoTCenterCoreBuilder AddIoTCenterCore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }


            if (services
                .LastOrDefault(d => d.ServiceType == typeof(IoTCenterCoreBuilder))?
                .ImplementationInstance is not IoTCenterCoreBuilder builder)
            {
                builder = new IoTCenterCoreBuilder(services);
                services.AddSingleton(builder);

                AddDefaultServices(services);

                AddShellServices(services);

                AddExtensionServices(builder);

                AddStaticFiles(builder);

                AddRouting(builder);

                AddSameSiteCookieBackwardsCompatibility(builder);

                AddAuthentication(builder);

                AddDataProtection(builder);

                services.AddSingleton(services);
            }

            builder.AddBackgroundService();

            return builder;
        }

        public static IServiceCollection AddIoTCenterCore(this IServiceCollection services, Action<IoTCenterCoreBuilder> configure)
        {
            var builder = services.AddIoTCenterCore();

            configure?.Invoke(builder);

            return services;
        }

        private static void AddDefaultServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            services.AddLocalization();

            services.AddWebEncoders();

            services.AddHttpContextAccessor();
            services.AddSingleton<IClock, Clock>();
            services.AddScoped<ILocalClock, LocalClock>();

            services.AddScoped<ILocalizationService, DefaultLocalizationService>();
            services.AddScoped<ICalendarManager, DefaultCalendarManager>();
            services.AddScoped<ICalendarSelector, DefaultCalendarSelector>();

            services.AddScoped<IIoTCenterHelper, DefaultIoTCenterHelper>();

            services.AddSingleton<IAssemblyFinder, AssemblyFinder>();
            services.AddSingleton<ITypeFinder, TypeFinder>();

            services.AddSingleton<IIoTConfiguration, IoTConfiguration>();

            services.AddSingleton<IActionDescriptorChangeProvider>(PluginApiDescriptorChangeProvider.Instance);
            services.AddSingleton(PluginApiDescriptorChangeProvider.Instance);
            services.AddSingleton<PluginApiActivate>();

            services.AddAutoMapper();
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton<IObjectMapping>(sp =>
            {
                var assemblyFinder = sp.GetRequiredService<IAssemblyFinder>();

                var applicationContext = sp.GetRequiredService<IApplicationContext>();

                var modules = applicationContext.Application.Modules;

                var defaultAssemblies = assemblyFinder.Assemblies;

                var pluginAssemblies = modules.Where(m => !defaultAssemblies.Any(d => d.GetName().Name == m.Name))
                .Select(d => d.Assembly).ToList();

                pluginAssemblies.AddRange(defaultAssemblies);

                var typeFinder = sp.GetRequiredService<ITypeFinder>();

                var types = typeFinder.Find<IAutoMapperConfig>(pluginAssemblies);

                var instances = types.Select(type => Reflection.CreateInstance<IAutoMapperConfig>(type)).ToList();

                var expression = new MapperConfigurationExpression();

                instances.ForEach(t => t.Config(expression));

                var configuration = new MapperConfiguration(expression);

                var mapper = new ObjectMapping(configuration);

                ObjectMapperExtensions.SetMapper(mapper);

                return mapper;
            });

            return services;
        }

        private static void AddShellServices(IServiceCollection services)
        {
            services.AddHostingShellServices();
            services.AddAllFeaturesDescriptor();

            services.AddTransient(sp => new ShellFeature
            (
                sp.GetRequiredService<IHostEnvironment>().ApplicationName, alwaysEnabled: true)
            );

            services.AddTransient(sp => new ShellFeature
            (
                Application.DefaultFeatureId, alwaysEnabled: true)
            );
        }

        private static void AddExtensionServices(IoTCenterCoreBuilder builder)
        {
            builder.ApplicationServices.AddSingleton<IPluginAssemblyProvider, PluginAssemblyProvider>();
            builder.ApplicationServices.AddSingleton<IModuleNamesProvider, AssemblyAttributeModuleNamesProvider>();
            builder.ApplicationServices.AddSingleton<IApplicationContext, ModularApplicationContext>();

            builder.ApplicationServices.AddExtensionManagerHost();

            builder.ConfigureServices(services =>
            {
                services.AddExtensionManager();
            });
        }

        private static void AddStaticFiles(IoTCenterCoreBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(serviceProvider =>
                {
                    var env = serviceProvider.GetRequiredService<IHostEnvironment>();
                    var appContext = serviceProvider.GetRequiredService<IApplicationContext>();

                    IModuleStaticFileProvider fileProvider;
                    if (env.IsDevelopment())
                    {
                        var fileProviders = new List<IStaticFileProvider>
                        {
                            new ModuleProjectStaticFileProvider(appContext),
                            new ModuleEmbeddedStaticFileProvider(appContext)
                        };
                        fileProvider = new ModuleCompositeStaticFileProvider(fileProviders);
                    }
                    else
                    {
                        fileProvider = new ModuleEmbeddedStaticFileProvider(appContext);
                    }
                    return fileProvider;
                });

                services.AddSingleton<IStaticFileProvider>(serviceProvider =>
                {
                    return serviceProvider.GetRequiredService<IModuleStaticFileProvider>();
                });
            });

            builder.Configure((app, routes, serviceProvider) =>
            {
                var fileProvider = serviceProvider.GetRequiredService<IModuleStaticFileProvider>();

                var options = serviceProvider.GetRequiredService<IOptions<StaticFileOptions>>().Value;

                options.RequestPath = "";
                options.FileProvider = fileProvider;

                var shellConfiguration = serviceProvider.GetRequiredService<IShellConfiguration>();

                var cacheControl = shellConfiguration.GetValue("StaticFileOptions:CacheControl", "public, max-age=2592000, s-max-age=31557600");

                options.OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = cacheControl;
                };

                app.UseStaticFiles(options);
            });
        }

        private static void AddRouting(IoTCenterCoreBuilder builder)
        {

            builder.ConfigureServices(collection =>
            {

                var implementationTypesToRemove = new ServiceCollection().AddRouting()
                    .Where(sd => sd.Lifetime == ServiceLifetime.Singleton || sd.ServiceType == typeof(IConfigureOptions<RouteOptions>))
                    .Select(sd => sd.GetImplementationType())
                    .ToArray();

                var descriptorsToRemove = collection
                    .Where(sd => (sd is ClonedSingletonDescriptor || sd.ServiceType == typeof(IConfigureOptions<RouteOptions>)) &&
                        implementationTypesToRemove.Contains(sd.GetImplementationType()))
                    .ToArray();

                foreach (var descriptor in descriptorsToRemove)
                {
                    collection.Remove(descriptor);
                }

                collection.AddRouting();
            },
            order: int.MinValue + 100);
        }

        private static void AddSameSiteCookieBackwardsCompatibility(IoTCenterCoreBuilder builder)
        {
            builder.ConfigureServices(services =>
                {
                    services.Configure<CookiePolicyOptions>(options =>
                    {
                        options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                        options.OnAppendCookie = cookieContext => CheckSameSiteBackwardsCompatiblity(cookieContext.Context, cookieContext.CookieOptions);
                        options.OnDeleteCookie = cookieContext => CheckSameSiteBackwardsCompatiblity(cookieContext.Context, cookieContext.CookieOptions);
                    });
                })
                .Configure(app =>
                {
                    app.UseCookiePolicy();
                });
        }

        private static void CheckSameSiteBackwardsCompatiblity(HttpContext httpContext, CookieOptions options)
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            if (options.SameSite == SameSiteMode.None)
            {
                if (string.IsNullOrEmpty(userAgent))
                {
                    return;
                }

                if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
                {
                    options.SameSite = AspNetCore.Http.SameSiteMode.Unspecified;
                    return;
                }

                if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                    userAgent.Contains("Version/") && userAgent.Contains("Safari"))
                {
                    options.SameSite = AspNetCore.Http.SameSiteMode.Unspecified;
                    return;
                }

                if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
                {
                    options.SameSite = AspNetCore.Http.SameSiteMode.Unspecified;
                }
            }
        }

        private static void AddAuthentication(IoTCenterCoreBuilder builder)
        {
            builder.ApplicationServices.AddAuthentication();

            builder.ConfigureServices(services =>
            {
                services.AddAuthentication();

                services.AddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
            })
            .Configure(app =>
            {
                app.UseAuthentication();
            });
        }

        private static void AddDataProtection(IoTCenterCoreBuilder builder)
        {
            builder.ConfigureServices((services, serviceProvider) =>
            {
                var settings = serviceProvider.GetRequiredService<ShellSettings>();
                var options = serviceProvider.GetRequiredService<IOptions<ShellOptions>>();

                var directory = new DirectoryInfo(Path.Combine(
                    options.Value.ShellsApplicationDataPath,
                    options.Value.ShellsContainerName,
                    settings.Name, "DataProtection-Keys"));

                var collection = new ServiceCollection()
                    .AddDataProtection()
                    .PersistKeysToFileSystem(directory)
                    .SetApplicationName(settings.Name)
                    .AddKeyManagementOptions(o => o.XmlEncryptor = o.XmlEncryptor ?? new NullXmlEncryptor())
                    .Services;

                services.RemoveAll<IConfigureOptions<KeyManagementOptions>>();
                services.RemoveAll<IConfigureOptions<DataProtectionOptions>>();

                services.Add(collection);
            });
        }
    }
}
