using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoTCenterCore.Modules
{
    public static class Reflection
    {

        public static T CreateInstance<T>(Type type, params object[] parameters)
        {
            return To<T>(Activator.CreateInstance(type, parameters));
        }

        public static T To<T>(object input)
        {
            var inputString = input.SafeString();
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return default;
            }

            var type = typeof(T);
            var typeName = type.Name.ToLower();
            try
            {
                if (typeName == "string")
                {
                    return (T)(object)inputString;
                }
                else if (typeName == "guid")
                {
                    return (T)(object)new Guid(inputString);
                }
                else if (type.IsEnum)
                {
                    return Parse<T>(input);
                }
                else if (input is IConvertible)
                {
                    return (T)Convert.ChangeType(input, type);
                }
                return (T)input;
            }
            catch
            {
                return default;
            }
        }

        private static TEnum Parse<TEnum>(object member)
        {
            string value = member.SafeString();
            if (string.IsNullOrWhiteSpace(value))
            {
                if (typeof(TEnum).IsGenericType)
                {
                    return default;
                }

                throw new ArgumentNullException(nameof(member));
            }

            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        private static string SafeString(this object input)
        {
            return input?.ToString()?.Trim() ?? string.Empty;
        }

        public static List<Type> FindImplementTypes(Type findType, Assembly assembly)
        {
            var result = new List<Type>();

            result.AddRange(GetTypes(findType, assembly));

            return result.Distinct().ToList();
        }

        private static List<Type> GetTypes(Type findType, Assembly assembly)
        {
            var result = new List<Type>();
            if (assembly == null)
            {
                return result;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                return result;
            }

            foreach (var type in types)
            {
                AddType(result, findType, type);
            }

            return result;
        }

        private static void AddType(List<Type> result, Type findType, Type type)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                return;
            }

            var name = type.Name;

            if (name.Equals("Startup") || name.Contains("Controller"))
            {
                return;
            }

            if (!findType.IsAssignableFrom(type) && !MatchGeneric(findType, type))
            {
                return;
            }
            result.Add(type);
        }

        private static bool MatchGeneric(Type findType, Type type)
        {
            if (!findType.IsGenericTypeDefinition)
            {
                return false;
            }

            var definition = findType.GetGenericTypeDefinition();

            foreach (var implementedInterface in type.FindInterfaces((filter, criteria) => true, null))
            {
                if (!implementedInterface.IsGenericType)
                {
                    continue;
                }
                return definition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
            }
            return false;
        }
    }
}
