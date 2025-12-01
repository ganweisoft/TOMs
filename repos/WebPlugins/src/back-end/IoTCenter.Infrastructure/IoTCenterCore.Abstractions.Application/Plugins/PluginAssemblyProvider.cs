using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace IoTCenterCore.Modules
{
    public class PluginAssemblyProvider : IPluginAssemblyProvider
    {
        private readonly ILogger<PluginAssemblyProvider> _logger;
        public PluginAssemblyProvider(IServiceProvider serviceProvider, ILogger<PluginAssemblyProvider> logger)
        {
            PluginLoader.ServiceProvider = serviceProvider;
            _logger = logger;
        }
        private const string PLUGIN_CONTAIN = "IoTCenter.Module";

        public IEnumerable<PluginLoader> GetPluginLoaders()
        {
            var baseDirectory = AppContext.BaseDirectory;

            var assembliesPath = Path.Combine(baseDirectory, Application.ModulesPath);

            if (!Directory.Exists(assembliesPath))
            {
                Directory.CreateDirectory(assembliesPath);
                yield return null;
            }

            foreach (var pluginfolder in Directory.EnumerateDirectories(assembliesPath))
            {
                var pluginName = Path.GetFileName(pluginfolder);

                foreach (var file in Directory.GetFiles(pluginfolder, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    var moduleName = Path.GetFileNameWithoutExtension(file);

                    if (!pluginName.Equals(moduleName))
                    {
                        continue;
                    }

                    if (!moduleName.Contains(PLUGIN_CONTAIN))
                    {
                        continue;
                    }

                    if (moduleName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    {
                        continue;
                    }

                    PluginLoader pluginLoader = null;

                    try
                    {
                        pluginLoader = PluginLoader.CreateFromAssemblyFile(file, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"加载插件【{moduleName}】异常：{ex.Message}");
                        continue;
                    }

                    yield return pluginLoader;
                }
            }
        }
    }
}
