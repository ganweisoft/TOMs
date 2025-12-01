using System.Collections.Generic;
using System.Threading.Tasks;
using IoTCenterCore.Environment.Shell.Builders;
using IoTCenterCore.Environment.Shell.Scope;

namespace IoTCenterCore.Environment.Shell
{
    public interface IShellHost : IShellDescriptorManagerEventHandler
    {
        Task InitializeAsync();

        Task<ShellContext> GetOrCreateShellContextAsync(ShellSettings settings);

        Task<ShellScope> GetScopeAsync(ShellSettings settings);

        Task UpdateShellSettingsAsync(ShellSettings settings);

        Task ReloadShellContextAsync(ShellSettings settings);

        Task ReleaseShellContextAsync(ShellSettings settings);

        IEnumerable<ShellContext> ListShellContexts();

        bool TryGetSettings(string name, out ShellSettings settings);

        IEnumerable<ShellSettings> GetAllSettings();
    }
}
