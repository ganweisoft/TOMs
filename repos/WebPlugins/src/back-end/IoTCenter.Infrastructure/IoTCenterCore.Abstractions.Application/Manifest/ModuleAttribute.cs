using System;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterCore.Modules.Manifest
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class ModuleAttribute : FeatureAttribute
    {
        public ModuleAttribute()
        {
        }

        public virtual string Type => "Module";
        public new bool Exists => Id != null;

        public new string Id { get; set; }

        public string Author { get; set; } = String.Empty;

        public string Website { get; set; } = String.Empty;

        public string Version { get; set; } = "0.0";

        public string[] Tags { get; set; } = Enumerable.Empty<string>().ToArray();

        public List<FeatureAttribute> Features { get; } = new List<FeatureAttribute>();
    }
}
