using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FolderFile
{
    public enum SubfolderType { No, This, All }

    public static class DirectoryInfoExtension
    {
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo dir, SubfolderType type)
        {
            if (type == SubfolderType.No) yield break;
            if (type == SubfolderType.This) type = SubfolderType.No;

            foreach (FileInfo file in dir.GetFiles()) yield return file;
            foreach (FileInfo file in EnumerateDirectories(dir, type).SelectMany(d => d.GetFiles()))
            {
                yield return file;
            }
        }

        public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo dir, SubfolderType type)
        {
            if (type == SubfolderType.No) yield break;

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                yield return subDir;

                if (type == SubfolderType.This) continue;

                foreach (DirectoryInfo subSubDir in EnumerateDirectories(subDir, SubfolderType.All))
                {
                    yield return subSubDir;
                }
            }
        }

        public static void OpenInExplorer(this DirectoryInfo dir)
        {
            Process.Start(dir.FullName);
        }

        public static void DeleteContent(this DirectoryInfo dir, SubfolderType type)
        {
            if (type == SubfolderType.No) return;

            if (type == SubfolderType.All)
            {
                foreach (DirectoryInfo subDir in EnumerateDirectories(dir, SubfolderType.This))
                {
                    DeleteContent(subDir, SubfolderType.All);
                    subDir.Delete(true);
                }
            }

            foreach (FileInfo file in EnumerateFiles(dir, SubfolderType.This))
            {
                file.Delete();
            }
        }

        public static long GetLength(this DirectoryInfo dir, SubfolderType type)
        {
            return EnumerateFiles(dir, type).Sum(f => f.Length);
        }
    }
}