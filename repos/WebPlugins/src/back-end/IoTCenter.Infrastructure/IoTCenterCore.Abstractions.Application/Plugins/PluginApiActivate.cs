using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCenterCore.Modules
{
    public sealed class PluginApiActivate
    {
        public PluginApiActivate(ApplicationPartManager manager, IApplicationContext applicationContext,
            ILogger<PluginApiActivate> logger)
        {
            _manager = manager;
            _logger = logger;
            _applicationContext = applicationContext;
        }

        private readonly IApplicationContext _applicationContext;
        private readonly ApplicationPartManager _manager;
        private readonly ILogger<PluginApiActivate> _logger;

        private bool _isInitialized = false;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public async Task Activator()
        {
            if (_isInitialized)
            {
                return;
            }

            await _semaphore.WaitAsync();

            try
            {
                if (_isInitialized)
                {
                    return;
                }

                var modules = _applicationContext.Application.Modules;

                foreach (var module in modules)
                {
                    ActivateApiController(module.Assembly);
                }
            }
            finally
            {
                _isInitialized = true;
                _semaphore.Release();
            }
        }

        public void ActivateApiController(Assembly assembly)
        {
            try
            {
                UnloadApiController(assembly.GetName().Name);

                _manager.ApplicationParts.Add(new AssemblyPart(assembly));

                PluginApiDescriptorChangeProvider.Instance.HasChanged = true;

                PluginApiDescriptorChangeProvider.Instance.TokenSource.Cancel();

                var controllerFeature = new ControllerFeature();

                _manager.PopulateFeature(controllerFeature);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"加载插件【{assembly.GetName().Name}】成功");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"加载插件【{assembly.GetName().Name}】失败，{ex}");

                UnloadApiController(assembly.GetName().Name);
            }
        }

        public void UnloadApiController(string plugin)
        {
            var unload = false;

            var index = -1;

            foreach (var part in _manager.ApplicationParts)
            {
                if (!part.Name.Equals(plugin, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                unload = true;

                index = _manager.ApplicationParts.IndexOf(part);
            }

            if (unload)
            {
                _manager.ApplicationParts.RemoveAt(index);

                PluginApiDescriptorChangeProvider.Instance.HasChanged = true;

                PluginApiDescriptorChangeProvider.Instance.TokenSource.Cancel();
            }
        }
    }
}
