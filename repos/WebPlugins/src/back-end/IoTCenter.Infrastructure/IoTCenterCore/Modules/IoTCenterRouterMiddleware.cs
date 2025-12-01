using IoTCenterCore.Environment.Shell.Builders;
using IoTCenterCore.Environment.Shell.Scope;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCenterCore.Modules
{
    public class IoTCenterRouterMiddleware
    {
        private readonly IFeatureCollection _features;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

        public IoTCenterRouterMiddleware(
            IFeatureCollection features,
            RequestDelegate next,
            ILogger<IoTCenterRouterMiddleware> logger)
        {
            _features = features;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var shellContext = ShellScope.Context;

            if (!string.IsNullOrEmpty(shellContext.Settings.RequestUrlPrefix))
            {
                PathString prefix = "/" + shellContext.Settings.RequestUrlPrefix;
                httpContext.Request.PathBase += prefix;
                httpContext.Request.Path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase, out PathString remainingPath);
                httpContext.Request.Path = remainingPath;
            }

            if (shellContext.Pipeline == null)
            {
                await InitializePipelineAsync(shellContext);
            }

            await shellContext.Pipeline.Invoke(httpContext);

            var pluginActivator = shellContext.ServiceProvider.GetRequiredService<PluginApiActivate>();

            await pluginActivator.Activator();
        }

        private async Task InitializePipelineAsync(ShellContext shellContext)
        {
            var semaphore = _semaphores.GetOrAdd(shellContext.Settings.Name, _ => new SemaphoreSlim(1));

            await semaphore.WaitAsync();

            try
            {
                if (shellContext.Pipeline == null)
                {
                    shellContext.Pipeline = BuildPipeline();
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private IShellPipeline BuildPipeline()
        {
            var appBuilder = new ApplicationBuilder(ShellScope.Context.ServiceProvider, _features);

            var startupFilters = appBuilder.ApplicationServices.GetService<IEnumerable<IStartupFilter>>();

            var shellPipeline = new ShellRequestPipeline();

            Action<IApplicationBuilder> configure = builder =>
            {
                ConfigurePipeline(builder);
            };

            foreach (var filter in startupFilters.Reverse())
            {
                configure = filter.Configure(configure);
            }

            configure(appBuilder);

            shellPipeline.Next = appBuilder.Build();

            return shellPipeline;
        }

        private void ConfigurePipeline(IApplicationBuilder appBuilder)
        {
            var startups = appBuilder.ApplicationServices.GetServices<IStartup>();

            startups = startups.OrderBy(s => s.ConfigureOrder);

            appBuilder.UseRouting().UseEndpoints(routes =>
            {
                foreach (var startup in startups)
                {
                    startup.Configure(appBuilder, routes, ShellScope.Services);
                }
            });
        }
    }
}
