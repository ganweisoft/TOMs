using System;

namespace IoTCenterCore.Modules
{
    public delegate void PluginReloadedEventHandler(object sender, PluginReloadedEventArgs eventArgs);

    public class PluginReloadedEventArgs : EventArgs
    {
        public PluginReloadedEventArgs(PluginLoader loader)
        {
            Loader = loader;
        }

        public PluginLoader Loader { get; }
    }
}