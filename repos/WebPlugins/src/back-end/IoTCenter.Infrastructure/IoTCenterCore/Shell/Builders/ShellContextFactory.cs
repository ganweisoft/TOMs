using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IoTCenterCore.Environment.Shell.Descriptor;
using IoTCenterCore.Environment.Shell.Descriptor.Models;

namespace IoTCenterCore.Environment.Shell.Builders
{
    public class ShellContextFactory : IShellContextFactory
    {
        private readonly ICompositionStrategy _compositionStrategy;
        private readonly IShellContainerFactory _shellContainerFactory;
        private readonly IEnumerable<ShellFeature> _shellFeatures;
        private readonly ILogger _logger;

        public ShellContextFactory(
            ICompositionStrategy compositionStrategy,
            IShellContainerFactory shellContainerFactory,
            IEnumerable<ShellFeature> shellFeatures,
            ILogger<ShellContextFactory> logger)
        {
            _compositionStrategy = compositionStrategy;
            _shellContainerFactory = shellContainerFactory;
            _shellFeatures = shellFeatures;
            _logger = logger;
        }

        async Task<ShellContext> IShellContextFactory.CreateShellContextAsync(ShellSettings settings)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Creating shell context for tenant '{TenantName}'", settings.Name);
            }

            var describedContext = await CreateDescribedContextAsync(settings, MinimumShellDescriptor());

            ShellDescriptor currentDescriptor;
            using (var scope = describedContext.ServiceProvider.CreateScope())
            {
                var shellDescriptorManager = scope.ServiceProvider.GetService<IShellDescriptorManager>();
                currentDescriptor = await shellDescriptorManager.GetShellDescriptorAsync();
            }

            if (currentDescriptor != null)
            {
                describedContext.Release();
                return await CreateDescribedContextAsync(settings, currentDescriptor);
            }

            return describedContext;
        }

        async Task<ShellContext> IShellContextFactory.CreateSetupContextAsync(ShellSettings settings)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("No shell settings available. Creating shell context for setup");
            }
            var descriptor = MinimumShellDescriptor();

            return await CreateDescribedContextAsync(settings, descriptor);
        }

        public async Task<ShellContext> CreateDescribedContextAsync(ShellSettings settings, ShellDescriptor shellDescriptor)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Creating described context for tenant '{TenantName}'", settings.Name);
            }

            await settings.EnsureConfigurationAsync();

            var blueprint = await _compositionStrategy.ComposeAsync(settings, shellDescriptor);
            var provider = _shellContainerFactory.CreateContainer(settings, blueprint);

            return new ShellContext
            {
                Settings = settings,
                Blueprint = blueprint,
                ServiceProvider = provider
            };
        }

        private ShellDescriptor MinimumShellDescriptor()
        {

            return new ShellDescriptor
            {
                SerialNumber = -1,
                Features = new List<ShellFeature>(_shellFeatures),
                Parameters = new List<ShellParameter>()
            };
        }
    }
}
