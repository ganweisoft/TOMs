using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IoTCenterCore.Modules.Loader
{
    [DebuggerDisplay("'{Name}' ({_mainAssemblyPath})")]
    internal class ManagedLoadContext : AssemblyLoadContext
    {
        public ILogger<ManagedLoadContext> Logger { get; set; }

        private readonly string _mainAssemblyPath;

        private readonly AssemblyDependencyResolver _dependencyResolver;

        private readonly AssemblyLoadContext _defaultAssemblyContext = GetLoadContext(Assembly.GetExecutingAssembly()) ?? Default;

        public ManagedLoadContext(string mainAssemblyPath,
            bool isCollectible)
            : base(Path.GetFileNameWithoutExtension(mainAssemblyPath), isCollectible)
        {
            _mainAssemblyPath = mainAssemblyPath ?? throw new ArgumentNullException(nameof(mainAssemblyPath));

            _dependencyResolver = new AssemblyDependencyResolver(mainAssemblyPath);

            Logger = NullLogger<ManagedLoadContext>.Instance;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName == null || string.IsNullOrEmpty(assemblyName.Name))
            {
                return null;
            }

            try
            {
                var defaultAssembly = _defaultAssemblyContext.LoadFromAssemblyName(assemblyName);

                return defaultAssembly;
            }
            catch (Exception ex) when (ex is FileLoadException || ex is FileNotFoundException)
            {
                var resolvedPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);

                if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath))
                {
                    return LoadAssemblyFromFilePath(resolvedPath);
                }
            }
            catch (Exception ex) when (ex is BadImageFormatException)
            {
                Logger.LogError($"Load【{assemblyName}】bad image format exception：{ex.StackTrace}\n{ex.Message}");
            }

            return null;
        }

        public Assembly LoadAssemblyFromFilePath(string path)
        {
            using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var pdbPath = Path.ChangeExtension(path, ".pdb");

            if (File.Exists(pdbPath))
            {
                using var pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return LoadFromStream(file, pdbFile);
            }

            return LoadFromStream(file);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var resolvedPath = _dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (!string.IsNullOrEmpty(resolvedPath) && File.Exists(resolvedPath))
            {
                return LoadUnmanagedDllFromResolvedPath(resolvedPath, normalizePath: false);
            }

            return base.LoadUnmanagedDll(unmanagedDllName);
        }

        private IntPtr LoadUnmanagedDllFromResolvedPath(string unmanagedDllPath, bool normalizePath = true)
        {
            if (normalizePath)
            {
                unmanagedDllPath = Path.GetFullPath(unmanagedDllPath);
            }

            return LoadUnmanagedDllFromPath(unmanagedDllPath);
        }
    }
}
