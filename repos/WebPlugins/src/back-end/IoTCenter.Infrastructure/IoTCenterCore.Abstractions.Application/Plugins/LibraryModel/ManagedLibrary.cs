using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace IoTCenterCore.Modules.LibraryModel
{
    [DebuggerDisplay("{Name} = {AdditionalProbingPath}")]
    public class ManagedLibrary
    {
        private ManagedLibrary(AssemblyName name, string additionalProbingPath, string appLocalPath)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AdditionalProbingPath = additionalProbingPath ?? throw new ArgumentNullException(nameof(additionalProbingPath));
            AppLocalPath = appLocalPath ?? throw new ArgumentNullException(nameof(appLocalPath));
        }

        public AssemblyName Name { get; private set; }

        public string AdditionalProbingPath { get; private set; }

        public string AppLocalPath { get; private set; }

        public static ManagedLibrary CreateFromPackage(string packageId, string packageVersion, string assetPath)
        {
            var appLocalPath = assetPath.StartsWith("lib/")
                ? Path.GetFileName(assetPath)
                : assetPath;

            return new ManagedLibrary(
                new AssemblyName(Path.GetFileNameWithoutExtension(assetPath)),
                Path.Combine(packageId.ToLowerInvariant(), packageVersion, assetPath),
                appLocalPath
            );
        }
    }
}
