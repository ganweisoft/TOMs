using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IoTCenterCore.Environment.Cache;
using IoTCenterCore.Environment.Shell.Builders;
using IoTCenterCore.Environment.Shell.Models;
using IoTCenterCore.Modules;

namespace IoTCenterCore.Environment.Shell.Scope
{
    public class ShellScope : IServiceScope
    {
        private static readonly AsyncLocal<ShellScope> _current = new AsyncLocal<ShellScope>();

        private static readonly Dictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();

        private readonly IServiceScope _serviceScope;

        private readonly Dictionary<object, object> _items = new Dictionary<object, object>();
        private readonly List<Func<ShellScope, Task>> _beforeDispose = new List<Func<ShellScope, Task>>();
        private readonly HashSet<string> _deferredSignals = new HashSet<string>();
        private readonly List<Func<ShellScope, Task>> _deferredTasks = new List<Func<ShellScope, Task>>();

        private bool _disposed = false;

        public ShellScope(ShellContext shellContext)
        {
            Interlocked.Increment(ref shellContext._refCount);

            ShellContext = shellContext;

            if (shellContext.ServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(shellContext.ServiceProvider),
                    $"Can't resolve a scope on tenant: {shellContext.Settings.Name}");
            }

            _serviceScope = shellContext.ServiceProvider.CreateScope();
            ServiceProvider = _serviceScope.ServiceProvider;
        }

        public ShellContext ShellContext { get; }
        public IServiceProvider ServiceProvider { get; }

        public static ShellContext Context => Current?.ShellContext;

        public static IServiceProvider Services => Current?.ServiceProvider;

        public static ShellScope Current => _current.Value;

        public static void Set(object key, object value) => Current._items[key] = value;

        public static object Get(object key) => Current._items.TryGetValue(key, out var value) ? value : null;

        public static T Get<T>(object key) => Current._items.TryGetValue(key, out var value) ? value is T item ? item : default : default;

        public static T GetOrCreate<T>(object key, Func<T> factory)
        {
            if (!Current._items.TryGetValue(key, out var value) || !(value is T item))
            {
                Current._items[key] = item = factory();
            }

            return item;
        }

        public static T GetOrCreate<T>(object key) where T : class, new()
        {
            if (!Current._items.TryGetValue(key, out var value) || !(value is T item))
            {
                Current._items[key] = item = new T();
            }

            return item;
        }

        public static void SetFeature<T>(T value) => Set(typeof(T), value);

        public static T GetFeature<T>() => Get<T>(typeof(T));

        public static T GetOrCreateFeature<T>(Func<T> factory) => GetOrCreate(typeof(T), factory);

        public static T GetOrCreateFeature<T>() where T : class, new() => GetOrCreate<T>(typeof(T));

        public static Task<ShellScope> CreateChildScopeAsync()
        {
            var shellHost = Services.GetRequiredService<IShellHost>();
            return shellHost.GetScopeAsync(Context.Settings);
        }

        public static Task<ShellScope> CreateChildScopeAsync(ShellSettings settings)
        {
            var shellHost = Services.GetRequiredService<IShellHost>();
            return shellHost.GetScopeAsync(settings);
        }

        public static Task<ShellScope> CreateChildScopeAsync(string tenant)
        {
            var shellHost = Services.GetRequiredService<IShellHost>();
            return shellHost.GetScopeAsync(tenant);
        }

        public static async Task UsingChildScopeAsync(Func<ShellScope, Task> execute)
        {
            await (await CreateChildScopeAsync()).UsingAsync(execute);
        }

        public static async Task UsingChildScopeAsync(ShellSettings settings, Func<ShellScope, Task> execute)
        {
            await (await CreateChildScopeAsync(settings)).UsingAsync(execute);
        }

        public static async Task UsingChildScopeAsync(string tenant, Func<ShellScope, Task> execute)
        {
            await (await CreateChildScopeAsync(tenant)).UsingAsync(execute);
        }

        public void StartAsyncFlow() => _current.Value = this;

        public async Task UsingAsync(Func<ShellScope, Task> execute)
        {
            if (Current == this)
            {
                await execute(Current);
                return;
            }

            using (this)
            {
                StartAsyncFlow();

                await ActivateShellAsync();

                await execute(this);

                await BeforeDisposeAsync();

                await DisposeAsync();
            }
        }

        public async Task ActivateShellAsync()
        {
            if (ShellContext.IsActivated)
            {
                return;
            }

            SemaphoreSlim semaphore;

            lock (_semaphores)
            {
                if (!_semaphores.TryGetValue(ShellContext.Settings.Name, out semaphore))
                {
                    _semaphores[ShellContext.Settings.Name] = semaphore = new SemaphoreSlim(1);
                }
            }

            await semaphore.WaitAsync();

            try
            {
                if (!ShellContext.IsActivated)
                {
                    using (var scope = ShellContext.CreateScope())
                    {
                        if (scope == null)
                        {
                            return;
                        }

                        scope.StartAsyncFlow();

                        var tenantEvents = scope.ServiceProvider.GetServices<IModularTenantEvents>();

                        foreach (var tenantEvent in tenantEvents)
                        {
                            await tenantEvent.ActivatingAsync();
                        }

                        foreach (var tenantEvent in tenantEvents.Reverse())
                        {
                            await tenantEvent.ActivatedAsync();
                        }

                        await scope.BeforeDisposeAsync();

                        await scope.DisposeAsync();
                    }

                    ShellContext.IsActivated = true;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void BeforeDispose(Func<ShellScope, Task> callback) => _beforeDispose.Insert(0, callback);

        private void DeferredSignal(string key) => _deferredSignals.Add(key);

        private void DeferredTask(Func<ShellScope, Task> task) => _deferredTasks.Add(task);

        public static void RegisterBeforeDispose(Func<ShellScope, Task> callback) => Current?.BeforeDispose(callback);

        public static void AddDeferredSignal(string key) => Current?.DeferredSignal(key);

        public static void AddDeferredTask(Func<ShellScope, Task> task) => Current?.DeferredTask(task);

        public async Task BeforeDisposeAsync()
        {
            foreach (var callback in _beforeDispose)
            {
                await callback(this);
            }

            if (_deferredSignals.Any())
            {
                var signal = ShellContext.ServiceProvider.GetRequiredService<ISignal>();

                foreach (var key in _deferredSignals)
                {
                    signal.SignalToken(key);
                }
            }

            if (_deferredTasks.Any())
            {
                var shellHost = ShellContext.ServiceProvider.GetRequiredService<IShellHost>();

                foreach (var task in _deferredTasks)
                {
                    ShellScope scope;

                    try
                    {
                        scope = await shellHost.GetScopeAsync(ShellContext.Settings);
                    }
                    catch
                    {
                        scope = new ShellScope(ShellContext);
                    }

                    using (scope)
                    {
                        scope.StartAsyncFlow();

                        var logger = scope.ServiceProvider.GetService<ILogger<ShellScope>>();

                        try
                        {
                            await task(scope);
                        }
                        catch (Exception e)
                        {
                            logger?.LogError(e,
                                "Error while processing deferred task '{TaskName}' on tenant '{TenantName}'.",
                                task.GetType().FullName, ShellContext.Settings.Name);
                        }

                        await scope.BeforeDisposeAsync();

                        await scope.DisposeAsync();
                    }
                }
            }
        }

        private async Task<bool> TerminateShellAsync()
        {
            if (ShellContext.Settings.State == TenantState.Disabled)
            {
                if (Interlocked.CompareExchange(ref ShellContext._refCount, 1, 1) == 1)
                {
                    ShellContext.Release();
                }
            }

            if (ShellContext._released && Interlocked.CompareExchange(ref ShellContext._refCount, 1, 1) == 1)
            {
                var tenantEvents = _serviceScope.ServiceProvider.GetServices<IModularTenantEvents>();

                foreach (var tenantEvent in tenantEvents)
                {
                    await tenantEvent.TerminatingAsync();
                }

                foreach (var tenantEvent in tenantEvents.Reverse())
                {
                    await tenantEvent.TerminatedAsync();
                }

                return true;
            }

            return false;
        }

        public async Task DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            var disposeShellContext = await TerminateShellAsync();

            _serviceScope.Dispose();

            if (disposeShellContext)
            {
                ShellContext.Dispose();
            }

            Interlocked.Decrement(ref ShellContext._refCount);

            _disposed = true;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
