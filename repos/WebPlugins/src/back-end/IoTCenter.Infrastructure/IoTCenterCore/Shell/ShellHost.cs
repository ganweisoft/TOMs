using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IoTCenterCore.Environment.Extensions;
using IoTCenterCore.Environment.Shell.Builders;
using IoTCenterCore.Environment.Shell.Descriptor.Models;
using IoTCenterCore.Environment.Shell.Models;
using IoTCenterCore.Environment.Shell.Scope;

namespace IoTCenterCore.Environment.Shell
{
    public class ShellHost : IShellHost, IDisposable
    {
        private const int ReloadShellMaxRetriesCount = 9;

        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IRunningShellTable _runningShellTable;
        private readonly IExtensionManager _extensionManager;
        private readonly ILogger _logger;

        private bool _initialized;
        private ConcurrentDictionary<string, ShellContext> _shellContexts = new ConcurrentDictionary<string, ShellContext>();
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _shellSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
        private SemaphoreSlim _initializingSemaphore = new SemaphoreSlim(1);

        public ShellHost(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,
            IRunningShellTable runningShellTable,
            IExtensionManager extensionManager,
            ILogger<ShellHost> logger)
        {
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;
            _runningShellTable = runningShellTable;
            _extensionManager = extensionManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            if (!_initialized)
            {
                await _initializingSemaphore.WaitAsync();
                try
                {
                    if (!_initialized)
                    {
                        await PreCreateAndRegisterShellsAsync();
                    }
                }
                finally
                {
                    _initialized = true;
                    _initializingSemaphore.Release();
                }
            }
        }

        public async Task<ShellContext> GetOrCreateShellContextAsync(ShellSettings settings)
        {
            ShellContext shell = null;

            while (shell == null)
            {
                if (!_shellContexts.TryGetValue(settings.Name, out shell))
                {
                    var semaphore = _shellSemaphores.GetOrAdd(settings.Name, (name) => new SemaphoreSlim(1));

                    await semaphore.WaitAsync();

                    try
                    {
                        if (!_shellContexts.TryGetValue(settings.Name, out shell))
                        {
                            shell = await CreateShellContextAsync(settings);
                            AddAndRegisterShell(shell);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                        _shellSemaphores.TryRemove(settings.Name, out semaphore);
                    }
                }

                if (shell.Released)
                {
                    _shellContexts.TryRemove(settings.Name, out var value);
                    shell = null;
                }
            }

            return shell;
        }

        public async Task<ShellScope> GetScopeAsync(ShellSettings settings)
        {
            ShellScope scope = null;

            while (scope == null)
            {
                if (!_shellContexts.TryGetValue(settings.Name, out var shellContext))
                {
                    shellContext = await GetOrCreateShellContextAsync(settings);
                }

                scope = shellContext.CreateScope();

                if (scope == null)
                {
                    _shellContexts.TryRemove(settings.Name, out var value);
                }
            }

            return scope;
        }

        public async Task UpdateShellSettingsAsync(ShellSettings settings)
        {
            settings.Identifier = IdGenerator.GenerateId();
            await _shellSettingsManager.SaveSettingsAsync(settings);
            await ReloadShellContextAsync(settings);
        }

        public Task ChangedAsync(ShellDescriptor descriptor, ShellSettings settings)
            => ReleaseShellContextAsync(settings);

        public async Task ReloadShellContextAsync(ShellSettings settings)
        {
            if (!CanReleaseShell(settings))
            {
                _runningShellTable.Remove(settings);
                return;
            }

            if (settings.State != TenantState.Initializing)
            {
                settings = await _shellSettingsManager.LoadSettingsAsync(settings.Name);
            }

            var count = 0;
            while (count < ReloadShellMaxRetriesCount)
            {
                count++;

                if (_shellContexts.TryRemove(settings.Name, out var context))
                {
                    _runningShellTable.Remove(settings);
                    context.Release();
                }

                if (!_shellContexts.TryAdd(settings.Name, new ShellContext.PlaceHolder { Settings = settings }))
                {
                    continue;
                }

                if (CanRegisterShell(settings))
                {
                    _runningShellTable.Add(settings);
                }

                if (settings.State == TenantState.Initializing)
                {
                    return;
                }

                var currentIdentifier = settings.Identifier;

                settings = await _shellSettingsManager.LoadSettingsAsync(settings.Name);

                if (settings.Identifier == currentIdentifier)
                {
                    return;
                }
            }

            throw new ShellHostReloadException(
                $"Unable to reload the tenant '{settings.Name}' as too many concurrent processes are trying to do so.");
        }

        public Task ReleaseShellContextAsync(ShellSettings settings)
        {
            if (!CanReleaseShell(settings))
            {
                return Task.CompletedTask;
            }

            if (_shellContexts.TryRemove(settings.Name, out var context))
            {
                context.Release();
            }

            _shellContexts.TryAdd(context.Settings.Name, new ShellContext.PlaceHolder { Settings = settings });

            return Task.CompletedTask;
        }

        public IEnumerable<ShellContext> ListShellContexts() => _shellContexts.Values.ToArray();

        public bool TryGetSettings(string name, out ShellSettings settings)
        {
            if (_shellContexts.TryGetValue(name, out var shell))
            {
                settings = shell.Settings;
                return true;
            }

            settings = null;
            return false;
        }

        public IEnumerable<ShellSettings> GetAllSettings() => ListShellContexts().Select(s => s.Settings);

        private async Task PreCreateAndRegisterShellsAsync()
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Start creation of shells");
            }

            var features = _extensionManager.LoadFeaturesAsync();

            var allSettings = (await _shellSettingsManager.LoadSettingsAsync()).Where(CanCreateShell).ToArray();
            var defaultSettings = allSettings.FirstOrDefault(s => s.Name == ShellHelper.DefaultShellName);
            var otherSettings = allSettings.Except(new[] { defaultSettings }).ToArray();

            await features;

            if (defaultSettings?.State != TenantState.Running)
            {
                var setupContext = await CreateSetupContextAsync(defaultSettings);
                AddAndRegisterShell(setupContext);
                allSettings = otherSettings;
            }

            if (allSettings.Length > 0)
            {
                foreach (var settings in allSettings)
                {
                    AddAndRegisterShell(new ShellContext.PlaceHolder { Settings = settings });
                };
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Done pre-creating and registering shells");
            }
        }

        private Task<ShellContext> CreateShellContextAsync(ShellSettings settings)
        {
            if (settings.State == TenantState.Uninitialized)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Creating shell context for tenant '{TenantName}' setup", settings.Name);
                }

                return _shellContextFactory.CreateSetupContextAsync(settings);
            }
            else if (settings.State == TenantState.Disabled)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Creating disabled shell context for tenant '{TenantName}'", settings.Name);
                }

                return Task.FromResult(new ShellContext { Settings = settings });
            }
            else if (settings.State == TenantState.Running || settings.State == TenantState.Initializing)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Creating shell context for tenant '{TenantName}'", settings.Name);
                }

                return _shellContextFactory.CreateShellContextAsync(settings);
            }
            else
            {
                throw new InvalidOperationException("Unexpected shell state for " + settings.Name);
            }
        }

        private Task<ShellContext> CreateSetupContextAsync(ShellSettings defaultSettings)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Creating shell context for root setup.");
            }

            if (defaultSettings == null)
            {
                var shellSettings = _shellSettingsManager.CreateDefaultSettings();
                shellSettings.Name = ShellHelper.DefaultShellName;
                shellSettings.State = TenantState.Uninitialized;
                defaultSettings = shellSettings;
            }

            return _shellContextFactory.CreateSetupContextAsync(defaultSettings);
        }

        private void AddAndRegisterShell(ShellContext context)
        {
            if (_shellContexts.TryAdd(context.Settings.Name, context) && CanRegisterShell(context))
            {
                RegisterShellSettings(context.Settings);
            }
        }

        private bool CanRegisterShell(ShellContext context)
        {
            if (!CanRegisterShell(context.Settings))
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Skipping shell context registration for tenant '{TenantName}'", context.Settings.Name);
                }

                return false;
            }

            return true;
        }

        private void RegisterShellSettings(ShellSettings settings)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Registering shell context for tenant '{TenantName}'", settings.Name);
            }

            _runningShellTable.Add(settings);
        }

        private bool CanCreateShell(ShellSettings shellSettings)
        {
            return
                shellSettings.State == TenantState.Running ||
                shellSettings.State == TenantState.Uninitialized ||
                shellSettings.State == TenantState.Initializing ||
                shellSettings.State == TenantState.Disabled;
        }

        private bool CanRegisterShell(ShellSettings shellSettings)
        {
            return
                shellSettings.State == TenantState.Running ||
                shellSettings.State == TenantState.Uninitialized ||
                shellSettings.State == TenantState.Initializing;
        }

        private bool CanReleaseShell(ShellSettings settings)
        {
            return settings.State != TenantState.Disabled || _shellContexts.TryGetValue(settings.Name, out var value) && value.ActiveScopes == 0;
        }

        public void Dispose()
        {
            foreach (var shell in ListShellContexts())
            {
                shell.Dispose();
            }
        }
    }
}
