using System;
using IoTCenterCore.Environment.Shell.Builders.Models;

namespace IoTCenterCore.Environment.Shell.Builders
{
    public interface IShellContainerFactory
    {
        IServiceProvider CreateContainer(ShellSettings settings, ShellBlueprint blueprint);
    }
}
