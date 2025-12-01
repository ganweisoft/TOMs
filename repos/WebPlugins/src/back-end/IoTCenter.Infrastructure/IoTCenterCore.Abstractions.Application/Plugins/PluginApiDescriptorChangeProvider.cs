using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace IoTCenterCore.Modules
{
    public class PluginApiDescriptorChangeProvider : IActionDescriptorChangeProvider
    {

        public static PluginApiDescriptorChangeProvider Instance { get; } = new PluginApiDescriptorChangeProvider();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
