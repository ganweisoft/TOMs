using System;
using System.IO;
using System.Runtime.Loader;

namespace IoTCenterCore.Modules.Loader
{
    public class AssemblyLoadContextBuilder
    {
        private string _mainAssemblyPath;

        private bool _isCollectible;

        public AssemblyLoadContext Build()
        {
            if (_mainAssemblyPath == null)
            {
                throw new InvalidOperationException($"Missing required property. You must call '{nameof(SetMainAssemblyPath)}' to configure the default assembly.");
            }

            return new ManagedLoadContext(_mainAssemblyPath, _isCollectible);
        }

        public AssemblyLoadContextBuilder SetMainAssemblyPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Argument must not be null or empty.", nameof(path));
            }

            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException("Argument must be a full path.", nameof(path));
            }

            _mainAssemblyPath = path;

            return this;
        }

        public AssemblyLoadContextBuilder EnableUnloading()
        {
            _isCollectible = true;
            return this;
        }
    }
}
