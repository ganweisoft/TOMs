using System.Collections.Generic;

namespace IoTCenterCore.Modules
{
    public interface IModuleNamesProvider
    {
        IEnumerable<string> GetModuleNames();
    }
}
