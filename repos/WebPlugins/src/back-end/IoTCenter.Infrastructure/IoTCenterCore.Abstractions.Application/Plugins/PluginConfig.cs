using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace IoTCenterCore.Modules
{
    public class PluginConfig
    {
        public PluginConfig(string mainAssemblyPath)
        {
            if (string.IsNullOrEmpty(mainAssemblyPath))
            {
                throw new ArgumentException("Value must be null or not empty", nameof(mainAssemblyPath));
            }

            if (!Path.IsPathRooted(mainAssemblyPath))
            {
                throw new ArgumentException("Value must be an absolute file path", nameof(mainAssemblyPath));
            }

            MainAssemblyPath = mainAssemblyPath;
        }

        public string MainAssemblyPath { get; }

        public ICollection<AssemblyName> PrivateAssemblies { get; protected set; } = new List<AssemblyName>();

        public bool IsLazyLoaded { get; set; } = false;

        public AssemblyLoadContext DefaultContext { get; set; } = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;


        private bool _isUnloadable;

        public bool IsUnloadable
        {
            get => _isUnloadable || EnableHotReload;
            set => _isUnloadable = value;
        }

        private bool _loadInMemory;

        public bool LoadInMemory
        {
            get => _loadInMemory || EnableHotReload;
            set => _loadInMemory = value;
        }

        public bool EnableHotReload { get; set; }

        public TimeSpan ReloadDelay { get; set; } = TimeSpan.FromMilliseconds(200);
    }
}
