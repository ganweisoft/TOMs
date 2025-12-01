using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;

namespace IoTCenterCore.Modules
{
    public interface IMigration
    {
        void Install();

        void Uninstall();
    }

    public static class ServiceProviderServiceExtension
    {
        public static IMigration GetMigrationRequiredService(this IServiceProvider provider, string pluginAssemblyName = null)
        {
            if (string.IsNullOrEmpty(pluginAssemblyName))
            {
                var stackTrace = new StackTrace();
                var assembly = stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly;
                pluginAssemblyName = assembly.GetName().Name;
            }

            var migrations = provider.GetServices<IMigration>()?.ToList();
            if (migrations == null || migrations.Count <= 0)
            {
                return default;
            }

            var migration = migrations.FirstOrDefault(m => m.GetType().Assembly.GetName().Name == pluginAssemblyName);

            return migration ?? default;
        }
    }
}
