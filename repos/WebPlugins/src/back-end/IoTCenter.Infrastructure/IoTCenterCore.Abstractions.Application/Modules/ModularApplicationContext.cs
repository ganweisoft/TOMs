using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTCenterCore.Modules
{
    public interface IApplicationContext
    {
        Application Application { get; }
    }

    public class ModularApplicationContext : IApplicationContext
    {
        private readonly IEnumerable<IPluginAssemblyProvider> _pluginProviders;
        private readonly IHostEnvironment _environment;
        private readonly IEnumerable<IModuleNamesProvider> _moduleNamesProviders;
        private Application _application;
        private static readonly object _initLock = new object();

        public ModularApplicationContext(IHostEnvironment environment,
            IEnumerable<IModuleNamesProvider> moduleNamesProviders,
            IEnumerable<IPluginAssemblyProvider> pluginProviders)
        {
            _environment = environment;
            _moduleNamesProviders = moduleNamesProviders;
            _pluginProviders = pluginProviders;
        }

        public Application Application
        {
            get
            {
                EnsureInitialized();
                return _application;
            }
        }

        private void EnsureInitialized()
        {
            if (_application == null)
            {
                lock (_initLock)
                {
                    if (_application == null)
                    {
                        _application = new Application(_environment, GetModules());
                    }
                }
            }
        }

        private IEnumerable<Module> GetModules()
        {
            var modules = new ConcurrentBag<Module>
            {
                new Module(_environment.ApplicationName, true)
            };

            var names = _moduleNamesProviders
                .SelectMany(p => p.GetModuleNames())
                .Where(n => n != _environment.ApplicationName)
                .Distinct();

            Parallel.ForEach(names, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (name) =>
            {
                modules.Add(new Module(name, false));
            });

            var pluginLoaders = _pluginProviders.SelectMany(p => p.GetPluginLoaders()).ToList();

            foreach (var pluginLoader in pluginLoaders)
            {
                var pluginAssembly = pluginLoader.LoadDefaultAssembly();

                var pluginAssmblyName = pluginAssembly.GetName().Name;

                if (modules.Any(m => m.Assembly.GetName().Name == pluginAssmblyName))
                {
                    continue;
                }

                modules.Add(new Module(pluginAssembly));
            }

            return modules;
        }
    }
}
