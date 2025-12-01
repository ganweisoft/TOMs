using IoTCenterCore.Modules;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public sealed class IoTConfiguration : IIoTConfiguration
    {
        private readonly IApplicationContext _context;
        private IConfigurationRoot _configuration;

        private readonly IConfigurationBuilder _configurationBuilder;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public IoTConfiguration(IConfiguration configuration,
            IApplicationContext context)
        {
            _context = context;

            _configurationBuilder = new ConfigurationBuilder().AddConfiguration(configuration);

            EnsureConfiguration();
        }

        private void EnsureConfiguration()
        {
            if (_configuration != null)
            {
                return;
            }

            try
            {
                _semaphore.Wait();

                if (_configuration != null)
                {
                    return;
                }

                var pluginPath = _context.Application.Modules
                  .Where(m => !string.IsNullOrEmpty(m.ModulePath))
                  .Select(m => m.ModulePath).ToList();

                Parallel.ForEach(pluginPath, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (path) =>
                {
                    LoadPluginConfiguration(path);
                });

                var providers = new List<IConfigurationProvider>();

                foreach (var source in _configurationBuilder.Sources)
                {
                    if (source is null)
                    {
                        continue;
                    }

                    var provider = source.Build(_configurationBuilder);

                    providers.Add(provider);
                }

                _configuration = new ConfigurationRoot(providers);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void LoadPluginConfiguration(string path)
        {
            var pluginPath = Path.ChangeExtension(path, "json");

            if (!File.Exists(pluginPath))
            {
                return;
            }

            _configurationBuilder.AddJsonFile(pluginPath, false, true);
        }

        private IConfiguration Configuration
        {
            get
            {
                EnsureConfiguration();

                return _configuration;
            }
        }

        public string this[string key]
        {
            get
            {
                var value = Configuration[key];

                return value ?? (key.Contains('_')
                    ? Configuration[key.Replace('_', '.')]
                    : null);
            }
            set
            {
                EnsureConfiguration();
            }
        }

        public IConfigurationSection GetSection(string key)
        {
            var section = Configuration.GetSection(key);

            return section.Exists()
                ? section
                : key.Contains('_')
                    ? Configuration.GetSection(key.Replace('_', '.'))
                    : section;
        }

        IEnumerable<IConfigurationSection> IConfiguration.GetChildren()
        {
            return Configuration.GetChildren();
        }

        IChangeToken IConfiguration.GetReloadToken()
        {
            return Configuration.GetReloadToken();
        }
    }
}
