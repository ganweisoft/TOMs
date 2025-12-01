using Microsoft.AspNetCore.Http;

namespace IoTCenterCore.Abstractions
{
    public interface IIoTCenterHelper
    {
        HttpContext HttpContext { get; }
    }
}
