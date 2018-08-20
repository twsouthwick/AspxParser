﻿using System.IO;
using System.Runtime.InteropServices;

namespace AspxParser
{
    public static class PathUtils
    {
        private static bool? isRunningOnLinux = null;
        private static bool? isCoreApp = null;

        public const int MaxDirLength = 248 - 1;
        public const int MaxPathLength = 260 - 1;

        public static bool IsWindows
        {
            get
            {
                if (!isRunningOnLinux.HasValue)
                {
                    isRunningOnLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                }

                return isRunningOnLinux.Value;
            }
        }

        public static bool IsCoreApp
        {
            get
            {
                if (!isCoreApp.HasValue)
                {
                    isCoreApp = RuntimeInformation.FrameworkDescription.Contains(".NET Core");
                }

                return isCoreApp.Value;
            }
        }

        public static string NormalizeFilePath(this string path) => NormalizePath(path, false);

        public static string NormalizeDirPath(this string path, bool force = false) => NormalizePath(path, true, force);

        private static string NormalizePath(this string path, bool isDirectory = true, bool force = false)
        {
            if (IsWindows && !IsCoreApp && !path.StartsWith(@"\\?\") &&
                (path.Length > (isDirectory ? MaxDirLength : MaxPathLength) || force))
            {
                if (path.StartsWith(@"\\"))
                {
                    return $@"\\?\UNC\{path.Remove(2)}";
                }

                path = path.NormalizeDirSeparator();

                return $@"\\?\{path}";
            }

            return path.NormalizeDirSeparator();
        }

        public static string NormalizeDirSeparator(this string path)
        {
            string notPlatformSeparator = IsWindows ? "/" : "\\";

            if (path.Contains(notPlatformSeparator))
            {
                return path.Replace(notPlatformSeparator, Path.DirectorySeparatorChar.ToString());
            }

            return path;
        }
    }
}
