using System;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace IoTCenterCore.Environment.Shell
{
    public class ShellOptionsSetup : IConfigureOptions<ShellOptions>
    {
        private const string IoTCenterAppData = "IoTCenter_APP_DATA";
        private const string DefaultAppDataPath = "App_Data";
        private const string DefaultSitesPath = "Sites";

        private readonly IHostEnvironment _hostingEnvironment;

        public ShellOptionsSetup(IHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void Configure(ShellOptions options)
        {
            var appData = System.Environment.GetEnvironmentVariable(IoTCenterAppData);

            if (!string.IsNullOrEmpty(appData))
            {
                options.ShellsApplicationDataPath = Path.Combine(_hostingEnvironment.ContentRootPath, appData);
            }
            else
            {
                options.ShellsApplicationDataPath = Path.Combine(_hostingEnvironment.ContentRootPath, DefaultAppDataPath);
            }

            options.ShellsContainerName = DefaultSitesPath;
        }
    }
}
