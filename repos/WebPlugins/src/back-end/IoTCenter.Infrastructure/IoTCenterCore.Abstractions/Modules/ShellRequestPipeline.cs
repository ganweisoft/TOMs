using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IoTCenterCore.Environment.Shell.Builders;

namespace IoTCenterCore.Modules
{
    public class ShellRequestPipeline : IShellPipeline
    {
        public RequestDelegate Next { get; set; }
        public Task Invoke(object context) => Next(context as HttpContext);
    }
}
