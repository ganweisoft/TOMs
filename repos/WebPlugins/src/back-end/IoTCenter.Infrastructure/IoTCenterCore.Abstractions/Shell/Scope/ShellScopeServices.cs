using System;

namespace IoTCenterCore.Environment.Shell.Scope
{
    public class ShellScopeServices : IServiceProvider
    {
        private readonly IServiceProvider _services;

        public ShellScopeServices(IServiceProvider services) => _services = services;

        private IServiceProvider Services => ShellScope.Services ?? _services;

        public object GetService(Type serviceType) => Services?.GetService(serviceType);
    }
}
