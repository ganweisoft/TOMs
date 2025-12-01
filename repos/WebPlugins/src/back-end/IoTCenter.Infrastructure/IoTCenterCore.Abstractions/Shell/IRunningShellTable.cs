using Microsoft.AspNetCore.Http;

namespace IoTCenterCore.Environment.Shell
{
    public interface IRunningShellTable
    {
        void Add(ShellSettings settings);
        void Remove(ShellSettings settings);
        ShellSettings Match(HostString host, PathString path, bool fallbackToDefault = true);
    }
}
