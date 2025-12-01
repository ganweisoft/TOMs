using System;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterCore.Modules.FileProviders
{
    public static class NormalizedPaths
    {
        public static void ResolveFolderContents(string folder, IEnumerable<string> normalizedPaths,
            out IEnumerable<string> filePaths, out IEnumerable<string> folderPaths)
        {
            var files = new HashSet<string>(StringComparer.Ordinal);
            var folders = new HashSet<string>(StringComparer.Ordinal);

            if (folder[folder.Length - 1] != '/')
            {
                folder = folder + '/';
            }

            foreach (var path in normalizedPaths.Where(a => a.StartsWith(folder, StringComparison.Ordinal)))
            {
                var subPath = path.Substring(folder.Length);
                var index = subPath.IndexOf('/');

                if (index == -1)
                {
                    files.Add(path);
                }
                else
                {
                    folders.Add(subPath.Substring(0, index));
                }
            }

            filePaths = files;
            folderPaths = folders;
        }
    }
}
