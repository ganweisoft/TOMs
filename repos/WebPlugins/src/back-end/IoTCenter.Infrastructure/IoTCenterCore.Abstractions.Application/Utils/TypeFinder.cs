using System;
using System.Collections.Generic;
using System.Reflection;

namespace IoTCenterCore.Modules
{
    public class TypeFinder : ITypeFinder
    {
        public List<Type> Find<T>(Assembly assembly)
        {
            return Find(typeof(T), assembly);
        }

        public List<Type> Find<T>(List<Assembly> assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(Find(typeof(T), assembly));
            }

            return types;
        }

        public List<Type> Find(Type findType, Assembly assembly)
        {
            return Reflection.FindImplementTypes(findType, assembly);
        }
    }
}
