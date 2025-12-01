using System;
using System.Collections.Generic;
using IoTCenterCore.Environment.Shell.Builders.Models;
using IoTCenterCore.Environment.Shell.Scope;

namespace IoTCenterCore.Environment.Shell.Builders
{
    public class ShellContext : IDisposable
    {
        private bool _disposed = false;
        internal volatile int _refCount = 0;
        internal bool _released = false;
        private List<WeakReference<ShellContext>> _dependents;
        private object _synLock = new object();

        public ShellSettings Settings { get; set; }
        public ShellBlueprint Blueprint { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public bool IsActivated { get; set; }

        public IShellPipeline Pipeline { get; set; }

        private bool _placeHolder;

        public class PlaceHolder : ShellContext
        {
            public PlaceHolder()
            {
                _placeHolder = true;
                _released = true;
                _disposed = true;
            }
        }

        public ShellScope CreateScope()
        {
            if (_placeHolder)
            {
                return null;
            }

            var scope = new ShellScope(this);

            if (!_released)
            {
                return scope;
            }

            scope.Dispose();

            return null;
        }

        public bool Released => _released;

        public int ActiveScopes => _refCount;

        public void Release()
        {
            if (_released == true)
            {
                return;
            }


            lock (_synLock)
            {
                if (_released == true)
                {
                    return;
                }

                _released = true;

                if (_dependents != null)
                {
                    foreach (var dependent in _dependents)
                    {
                        if (dependent.TryGetTarget(out var shellContext))
                        {
                            shellContext.Release();
                        }
                    }
                }

                if (_refCount == 0)
                {
                    Dispose();
                }
            }
        }

        public void AddDependentShell(ShellContext shellContext)
        {
            if (shellContext.Released)
            {
                return;
            }

            if (_released)
            {
                shellContext.Release();
                return;
            }

            lock (_synLock)
            {
                if (_dependents == null)
                {
                    _dependents = new List<WeakReference<ShellContext>>();
                }

                _dependents.RemoveAll(x => !x.TryGetTarget(out var shell) || shell.Settings.Name == shellContext.Settings.Name);

                _dependents.Add(new WeakReference<ShellContext>(shellContext));
            }
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            if (_disposed)
            {
                return;
            }

            if (ServiceProvider != null)
            {
                (ServiceProvider as IDisposable)?.Dispose();
                ServiceProvider = null;
            }

            IsActivated = false;
            Blueprint = null;
            Pipeline = null;

            _disposed = true;
        }

        ~ShellContext()
        {
            Close();
        }
    }
}
