using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using IoTCenterCore.Environment.Shell;
using IoTCenterCore.Environment.Shell.Configuration;
using IoTCenterCore.Environment.Shell.Descriptor;
using IoTCenterCore.Environment.Shell.Descriptor.Models;
using IoTCenterCore.Environment.Shell.Descriptor.Settings;
using IoTCenterCore.Modules;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class IoTCenterCoreBuilderExtensions
    {
        public static IoTCenterCoreBuilder AddGlobalFeatures(this IoTCenterCoreBuilder builder, params string[] featureIds)
        {
            foreach (var featureId in featureIds)
            {
                builder.ApplicationServices.AddTransient(sp => new ShellFeature(featureId, alwaysEnabled: true));
            }

            return builder;
        }

        public static IoTCenterCoreBuilder AddTenantFeatures(this IoTCenterCoreBuilder builder, params string[] featureIds)
        {
            builder.ConfigureServices(services =>
            {
                foreach (var featureId in featureIds)
                {
                    services.AddTransient(sp => new ShellFeature(featureId, alwaysEnabled: true));
                }
            });

            return builder;
        }

        public static IoTCenterCoreBuilder AddSetupFeatures(this IoTCenterCoreBuilder builder, params string[] featureIds)
        {
            foreach (var featureId in featureIds)
            {
                builder.ApplicationServices.AddTransient(sp => new ShellFeature(featureId));
            }

            return builder;
        }

        public static IoTCenterCoreBuilder WithTenants(this IoTCenterCoreBuilder builder)
        {
            var services = builder.ApplicationServices;

            services.AddSingleton<IShellsSettingsSources, ShellsSettingsSources>();
            services.AddSingleton<IShellsConfigurationSources, ShellsConfigurationSources>();
            services.AddSingleton<IShellConfigurationSources, ShellConfigurationSources>();
            services.AddTransient<IConfigureOptions<ShellOptions>, ShellOptionsSetup>();
            services.AddSingleton<IShellSettingsManager, ShellSettingsManager>();

            return builder.ConfigureServices(s =>
            {
                s.AddScoped<IShellDescriptorManager, ConfiguredFeaturesShellDescriptorManager>();
            });
        }

        public static IoTCenterCoreBuilder WithFeatures(this IoTCenterCoreBuilder builder, params string[] featureIds)
        {
            foreach (var featureId in featureIds)
            {
                builder.ApplicationServices.AddTransient(sp => new ShellFeature(featureId));
            }

            builder.ApplicationServices.AddSetFeaturesDescriptor();

            return builder;
        }

        public static IoTCenterCoreBuilder AddBackgroundService(this IoTCenterCoreBuilder builder)
        {
            builder.ApplicationServices.AddSingleton<IHostedService, ModularBackgroundService>();

            return builder;
        }
    }
}
