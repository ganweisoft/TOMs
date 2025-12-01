using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using IoTCenterCore.Modules;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseIoTCenterCore(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
            var appContext = app.ApplicationServices.GetRequiredService<IApplicationContext>();

            env.ContentRootFileProvider = new CompositeFileProvider(
                new ModuleEmbeddedFileProvider(appContext),
                env.ContentRootFileProvider);

            app.ApplicationServices.GetRequiredService<IWebHostEnvironment>()
                .ContentRootFileProvider = env.ContentRootFileProvider;

            app.UseMiddleware<IoTCenterContainerMiddleware>();

            configure?.Invoke(app);

            app.UseMiddleware<IoTCenterRouterMiddleware>(app.ServerFeatures);

            return app;
        }
    }
}
