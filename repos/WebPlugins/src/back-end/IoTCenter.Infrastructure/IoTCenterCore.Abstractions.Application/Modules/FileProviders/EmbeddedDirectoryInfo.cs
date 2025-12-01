using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace IoTCenterCore.Modules.FileProviders
{
    public class EmbeddedDirectoryInfo : IFileInfo
    {
        public EmbeddedDirectoryInfo(string name)
        {
            Name = name;
        }

        public bool Exists => true;

        public long Length => -1;

        public string PhysicalPath => null;

        public string Name { get; }

        public DateTimeOffset LastModified => DateTimeOffset.MinValue;

        public bool IsDirectory => true;

        public Stream CreateReadStream()
        {
            throw new InvalidOperationException("Cannot create a stream for a directory.");
        }
    }
}
