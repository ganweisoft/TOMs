using IoTCenterCore.Modules.Internal;
using IoTCenterCore.Modules.Loader;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IoTCenterCore.Modules
{
    public class PluginLoader : IDisposable
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, bool isUnloadable)
            => CreateFromAssemblyFile(assemblyFile, isUnloadable, _ => { });

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, bool isUnloadable, Action<PluginConfig> configure)
        {
            return CreateFromAssemblyFile(assemblyFile,
                    config =>
                    {
                        config.IsUnloadable = isUnloadable;
                        configure(config);
                    });
        }

        public static PluginLoader CreateFromAssemblyFile(string assemblyFile, Action<PluginConfig> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var config = new PluginConfig(assemblyFile);
            configure(config);
            return new PluginLoader(config);
        }

        private readonly PluginConfig _config;
        private ManagedLoadContext _context;
        private readonly AssemblyLoadContextBuilder _contextBuilder;
        private volatile bool _disposed;

        private FileSystemWatcher _fileWatcher;
        private Debouncer _debouncer;

        public PluginLoader(PluginConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _contextBuilder = CreateLoadContextBuilder(config);

            _context = (ManagedLoadContext)_contextBuilder.Build();

            if (config.EnableHotReload)
            {
                StartFileWatcher();
            }
        }

        public bool IsUnloadable
        {
            get
            {
                return _context.IsCollectible;
            }
        }

        public event PluginReloadedEventHandler Reloaded;

        public void Reload()
        {
            EnsureNotDisposed();

            if (!IsUnloadable)
            {
                throw new InvalidOperationException("Reload cannot be used because IsUnloadable is false");
            }

            _context.Unload();

            _context = (ManagedLoadContext)_contextBuilder.Build();

            GC.Collect();

            GC.WaitForPendingFinalizers();

            Reloaded?.Invoke(this, new PluginReloadedEventArgs(this));

            if (!File.Exists(_config.MainAssemblyPath))
            {
                return;
            }

            var pluginAssembly = _context.LoadAssemblyFromFilePath(_config.MainAssemblyPath);

            var pluginApiControllerActivate = ServiceProvider.GetRequiredService<PluginApiActivate>();

            pluginApiControllerActivate.ActivateApiController(pluginAssembly);
        }

        private void StartFileWatcher()
        {
            /*
            This is a very simple implementation.
            Some improvements that could be made in the future:

                * Watch all directories which contain assemblies that could be loaded
                * Support a polling file watcher.
                * Handle delete/recreate better.

            If you're interested in making improvements, feel free to send a pull request.
            */

            _debouncer = new Debouncer(_config.ReloadDelay);

            _fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(_config.MainAssemblyPath)
            };
            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.Filter = "*.dll";
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!_disposed)
            {
                _debouncer?.Execute(Reload);
            }
        }

        internal AssemblyLoadContext LoadContext => _context;

        public Assembly LoadDefaultAssembly()
        {
            EnsureNotDisposed();
            return _context.LoadAssemblyFromFilePath(_config.MainAssemblyPath);
        }

        public Assembly LoadAssembly(AssemblyName assemblyName)
        {
            EnsureNotDisposed();
            return _context.LoadFromAssemblyName(assemblyName);
        }

        public Assembly LoadAssemblyFromPath(string assemblyPath)
            => _context.LoadAssemblyFromFilePath(assemblyPath);

        public Assembly LoadAssembly(string assemblyName)
        {
            EnsureNotDisposed();
            return LoadAssembly(new AssemblyName(assemblyName));
        }

#if !NETCOREAPP2_1
        public AssemblyLoadContext.ContextualReflectionScope EnterContextualReflection()
            => _context.EnterContextualReflection();
#endif

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnFileChanged;
                _fileWatcher.Dispose();
            }

            _debouncer?.Dispose();

            if (_context.IsCollectible)
            {
                _context.Unload();
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PluginLoader));
            }
        }

        private static AssemblyLoadContextBuilder CreateLoadContextBuilder(PluginConfig config)
        {
            var builder = new AssemblyLoadContextBuilder();

            builder.SetMainAssemblyPath(config.MainAssemblyPath);

            if (config.IsUnloadable || config.EnableHotReload)
            {
                builder.EnableUnloading();
            }

            return builder;
        }
    }
}
