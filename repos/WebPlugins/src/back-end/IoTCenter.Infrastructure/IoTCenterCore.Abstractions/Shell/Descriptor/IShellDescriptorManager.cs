using System.Collections.Generic;
using System.Threading.Tasks;
using IoTCenterCore.Environment.Shell.Descriptor.Models;

namespace IoTCenterCore.Environment.Shell.Descriptor
{
    public interface IShellDescriptorManager
    {
        Task<ShellDescriptor> GetShellDescriptorAsync();

        Task UpdateShellDescriptorAsync(
            int priorSerialNumber,
            IEnumerable<ShellFeature> enabledFeatures,
            IEnumerable<ShellParameter> parameters);
    }
}
