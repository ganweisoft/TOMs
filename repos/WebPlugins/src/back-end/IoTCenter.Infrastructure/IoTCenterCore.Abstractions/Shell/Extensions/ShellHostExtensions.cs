using System;
using System.Threading.Tasks;
using IoTCenterCore.Environment.Shell.Scope;

namespace IoTCenterCore.Environment.Shell
{
    public static class ShellHostExtensions
    {
        public static Task<ShellScope> GetScopeAsync(this IShellHost shellHost, string tenant)
        {
            return shellHost.GetScopeAsync(shellHost.GetSettings(tenant));
        }

        public async static Task ReloadAllShellContextsAsync(this IShellHost shellHost)
        {
            foreach (var shell in shellHost.ListShellContexts())
            {
                await shellHost.ReloadShellContextAsync(shell.Settings);
            }
        }

        public async static Task ReleaseAllShellContextsAsync(this IShellHost shellHost)
        {
            foreach (var shell in shellHost.ListShellContexts())
            {
                await shellHost.ReleaseShellContextAsync(shell.Settings);
            }
        }

        public static ShellSettings GetSettings(this IShellHost shellHost, string tenant)
        {
            if (!shellHost.TryGetSettings(tenant, out var settings))
            {
                throw new ArgumentException("The specified tenant name is not valid.", nameof(tenant));
            }

            return settings;
        }
    }
}
