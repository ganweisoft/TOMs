using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTCenterCore.Environment.Shell.Descriptor.Models;

namespace IoTCenterCore.Environment.Shell.Descriptor.Settings
{
    public class SetFeaturesShellDescriptorManager : IShellDescriptorManager
    {
        private readonly IEnumerable<ShellFeature> _shellFeatures;
        private ShellDescriptor _shellDescriptor;

        public SetFeaturesShellDescriptorManager(IEnumerable<ShellFeature> shellFeatures)
        {
            _shellFeatures = shellFeatures;
        }

        public Task<ShellDescriptor> GetShellDescriptorAsync()
        {
            if (_shellDescriptor == null)
            {
                _shellDescriptor = new ShellDescriptor
                {
                    Features = _shellFeatures.Distinct().ToList()
                };
            }

            return Task.FromResult(_shellDescriptor);
        }

        public Task UpdateShellDescriptorAsync(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
        {
            return Task.CompletedTask;
        }
    }
}
