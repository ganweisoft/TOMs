using System.Threading.Tasks;
using IoTCenterCore.Environment.Shell.Descriptor.Models;

namespace IoTCenterCore.Environment.Shell.Builders
{
    public interface IShellContextFactory
    {
        Task<ShellContext> CreateShellContextAsync(ShellSettings settings);

        Task<ShellContext> CreateSetupContextAsync(ShellSettings settings);

        Task<ShellContext> CreateDescribedContextAsync(ShellSettings settings, ShellDescriptor shellDescriptor);
    }
}
