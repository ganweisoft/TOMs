using System;
using Microsoft.AspNetCore.Http;
using IoTCenterCore.Environment.Shell;

namespace IoTCenterCore.Modules
{
    public static class RunningShellTableExtensions
    {
        public static ShellSettings Match(this IRunningShellTable table, HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var httpRequest = httpContext.Request;


            return table.Match(httpRequest.Host, httpRequest.Path, true);
        }
    }
}
