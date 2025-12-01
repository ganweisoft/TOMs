using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using IoTCenterCore.Environment.Shell;
using IoTCenterCore.Environment.Shell.Models;

namespace IoTCenterCore.Modules
{
    public class IoTCenterContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IShellHost _shellHost;
        private readonly IRunningShellTable _runningShellTable;

        public IoTCenterContainerMiddleware(
            RequestDelegate next,
            IShellHost shellHost,
            IRunningShellTable runningShellTable)
        {
            _next = next;
            _shellHost = shellHost;
            _runningShellTable = runningShellTable;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _shellHost.InitializeAsync();

            var shellSettings = _runningShellTable.Match(httpContext);

            if (shellSettings != null)
            {
                if (shellSettings.State == TenantState.Initializing)
                {
                    httpContext.Response.Headers.Add(HeaderNames.RetryAfter, "10");
                    httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    await httpContext.Response.WriteAsync("The requested is currently initializing.");
                    return;
                }

                httpContext.UseShellScopeServices();

                var shellScope = await _shellHost.GetScopeAsync(shellSettings);

                httpContext.Features.Set(new ShellContextFeature
                {
                    ShellContext = shellScope.ShellContext,
                    OriginalPath = httpContext.Request.Path,
                    OriginalPathBase = httpContext.Request.PathBase
                });

                await shellScope.UsingAsync(scope => _next.Invoke(httpContext));
            }
        }
    }
}
