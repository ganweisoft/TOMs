using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoTCenterCore.Modules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequireFeaturesAttribute : Attribute
    {
        public RequireFeaturesAttribute(string featureName)
        {
            RequiredFeatureNames = new string[] { featureName };
        }

        public RequireFeaturesAttribute(string featureName, params string[] otherFeatureNames)
        {
            var list = new List<string>(otherFeatureNames)
            {
                featureName
            };

            RequiredFeatureNames = list;
        }

        public IList<string> RequiredFeatureNames { get; }

        public static IList<string> GetRequiredFeatureNamesForType(Type type)
        {
            var attribute = type.GetCustomAttributes<RequireFeaturesAttribute>(false).FirstOrDefault();

            return attribute?.RequiredFeatureNames ?? Array.Empty<string>();
        }
    }
}
