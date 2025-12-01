using System.Collections.Generic;

namespace IoTCenterCore.Environment.Shell.Descriptor.Models
{
    public class ShellDescriptor
    {
        public int SerialNumber { get; set; }

        public IList<ShellFeature> Features { get; set; } = new List<ShellFeature>();

        public IList<ShellParameter> Parameters { get; set; } = new List<ShellParameter>();
    }
}
