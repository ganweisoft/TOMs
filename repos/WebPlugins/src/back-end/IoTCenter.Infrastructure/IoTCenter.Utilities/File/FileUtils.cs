// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.IO;
using System.Linq;

namespace IoTCenter.Utilities
{

    public static class FileUtils
    {

        #region StreamFunctions
        public static void CopyStream(Stream source, Stream dest, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                dest.Write(buffer, 0, read);
            }
        }

        public static void CopyStream(Stream source, Stream dest, int bufferSize, bool append)
        {
            if (append)
                dest.Seek(0, SeekOrigin.End);

            CopyStream(source, dest, bufferSize);
        }

        #endregion

        #region 文件和路径转换

        public static string SafeFilename(string fileName, string replacementString = "", string spaceReplacement = null)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            string file = Path.GetInvalidFileNameChars()
                .Aggregate(fileName.Trim(),
                    (current, c) => current.Replace(c.ToString(), replacementString));

            file = file.Replace("#", "");

            if (!string.IsNullOrEmpty(spaceReplacement))
                file = file.Replace(" ", spaceReplacement);

            return file.Trim();
        }

        public static string CamelCaseSafeFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return filename;

            string fname = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);

            return StringUtils.ToCamelCase(SafeFilename(fname)) + ext;
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            char slash = Path.DirectorySeparatorChar;
            path = path.Replace('/', slash).Replace('\\', slash);
            return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
        }

        public static string NormalizePathDiagonalBar(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            char slash = Path.DirectorySeparatorChar;
            path = path.Replace(slash, '/');
            return path;
        }


	    public static string NormalizeDirectory(string path)
        {
            path = NormalizePath(path);
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                path += Path.DirectorySeparatorChar;
            return path;
        }


	    public static string AddTrailingSlash(string path)
        {
            string separator = Path.DirectorySeparatorChar.ToString();

            path = path.TrimEnd();

            if (path.EndsWith(separator) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                return path;

            return path + separator;
        }
        #endregion

        #region Miscellaneous functions

        public static void CopyDirectory(string sourcePath, string targetPath, bool deleteFirst = false, bool deepCopy = true)
        {
            if (deleteFirst && Directory.Exists(targetPath))
                Directory.Delete(targetPath, true);

            var searchOption = SearchOption.TopDirectoryOnly;
            if (deepCopy)
                searchOption = SearchOption.AllDirectories;

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", searchOption))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", searchOption))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }


        public static int DeleteFiles(string path, string filespec, bool recursive)
        {
            int failed = 0;
            path = Path.GetFullPath(path);
            string spec = Path.GetFileName(filespec);
            string[] files = Directory.GetFiles(path, spec);

            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    failed++;
                }
            }

            if (recursive)
            {
                var dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    failed += DeleteFiles(dir, filespec, recursive);
                }
            }


            return failed;
        }

        public static void DeleteTimedoutFiles(string filespec, int seconds)
        {
            string path = Path.GetDirectoryName(filespec);
            string spec = Path.GetFileName(filespec);
            string[] files = Directory.GetFiles(path, spec);

            foreach (string file in files)
            {
                try
                {
                    if (File.GetLastWriteTimeUtc(file) < DateTime.UtcNow.AddSeconds(seconds * -1))
                        File.Delete(file);
                }
                catch
                {
                }
            }
        }
        #endregion

     

    }
}
