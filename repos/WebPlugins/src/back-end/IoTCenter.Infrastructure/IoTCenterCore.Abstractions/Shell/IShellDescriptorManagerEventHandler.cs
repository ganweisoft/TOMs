using System.Threading.Tasks;
using IoTCenterCore.Environment.Shell.Descriptor.Models;

namespace IoTCenterCore.Environment.Shell
{
    public interface IShellDescriptorManagerEventHandler
    {
        Task ChangedAsync(ShellDescriptor descriptor, ShellSettings settings);
    }
}
