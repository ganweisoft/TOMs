using System.Collections.Generic;

namespace IoTCenterCore.Modules
{
    public interface IPluginAssemblyProvider
    {
        IEnumerable<PluginLoader> GetPluginLoaders();
    }
}
