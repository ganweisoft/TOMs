using Microsoft.AspNetCore.Http;

namespace IoTCenterCore.Modules.FileProviders
{
    public interface IVirtualPathBaseProvider
    {
        PathString VirtualPathBase { get; }
    }
}
