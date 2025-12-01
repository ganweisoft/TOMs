using System.Collections.Generic;

namespace IoTCenterCore.Environment.Shell.State
{
    public class ShellState
    {
        public List<ShellFeatureState> Features { get; } = new List<ShellFeatureState>();
    }
}
