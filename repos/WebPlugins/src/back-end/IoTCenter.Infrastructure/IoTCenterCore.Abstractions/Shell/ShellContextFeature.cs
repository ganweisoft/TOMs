using Microsoft.AspNetCore.Http;
using IoTCenterCore.Environment.Shell.Builders;

namespace IoTCenterCore.Environment.Shell
{
    public class ShellContextFeature
    {
        public ShellContext ShellContext { get; set; }

        public PathString OriginalPathBase { get; set; }

        public PathString OriginalPath { get; set; }
    }
}
