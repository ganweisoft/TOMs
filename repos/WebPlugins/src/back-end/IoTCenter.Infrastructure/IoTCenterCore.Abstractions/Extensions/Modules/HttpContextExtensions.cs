using Microsoft.AspNetCore.Http;
using IoTCenterCore.Environment.Shell.Scope;

namespace IoTCenterCore.Modules
{
    public static class HttpContextExtensions
    {
        public static HttpContext UseShellScopeServices(this HttpContext httpContext)
        {
            httpContext.RequestServices = new ShellScopeServices(httpContext.RequestServices);
            return httpContext;
        }
    }
}
