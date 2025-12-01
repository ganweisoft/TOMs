using System;
using System.Collections.Generic;
using System.Reflection;

namespace IoTCenterCore.Modules
{
    public interface ITypeFinder
    {
        List<Type> Find<T>(Assembly assembly);
        List<Type> Find<T>(List<Assembly> assemblies);
        List<Type> Find(Type findType, Assembly assembly);
    }
}
