using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace IoTCenterCore.Modules
{
    public class AssemblyFinder : IAssemblyFinder
    {
        private readonly Lazy<IReadOnlyList<Assembly>> _assemblies;

        public AssemblyFinder()
        {
            _assemblies = new Lazy<IReadOnlyList<Assembly>>(FindDefaultAll, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public IReadOnlyList<Assembly> Assemblies => _assemblies.Value;

        public IReadOnlyList<Assembly> FindDefaultAll()
        {
            var assemblies = LoadAssemblies(AppContext.BaseDirectory, SearchOption.TopDirectoryOnly);

            return assemblies.Distinct().ToImmutableList();
        }

        private List<Assembly> LoadAssemblies(string folderPath, SearchOption searchOption)
        {
            return GetAssemblyFiles(folderPath, searchOption)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath).ToList();
        }

        private IEnumerable<string> GetAssemblyFiles(string folderPath, SearchOption searchOption)
        {
            return Directory
                .EnumerateFiles(folderPath, "*.dll*", searchOption)
                .Where(f => Path.GetFileNameWithoutExtension(f).StartsWith("IoTCenter"));
        }
    }
}
