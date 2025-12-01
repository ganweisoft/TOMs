using System.Collections.Generic;
using System.IO;
using System.Linq;
using IoTCenterCore.Environment.Extensions.Features;
using IoTCenterCore.Environment.Extensions.Manifests;

namespace IoTCenterCore.Environment.Extensions
{
    public class InternalExtensionInfo : IExtensionInfo
    {
        public InternalExtensionInfo(string subPath)
        {
            Id = Path.GetFileName(subPath);
            SubPath = subPath;

            Manifest = new NotFoundManifestInfo(subPath);
            Features = Enumerable.Empty<IFeatureInfo>();
        }

        public string Id { get; }
        public string SubPath { get; }
        public IManifestInfo Manifest { get; }
        public IEnumerable<IFeatureInfo> Features { get; }
        public bool Exists => Manifest.Exists;
    }
}
