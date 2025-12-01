using System.Collections.Generic;
using System.Reflection;

namespace IoTCenterCore.Modules
{
    public interface IAssemblyFinder
    {
        IReadOnlyList<Assembly> Assemblies { get; }
    }
}
