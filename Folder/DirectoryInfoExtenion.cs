using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FolderFile
{
    public enum SubdirectoryType { No, This, All }

    public static class DirectoryInfoExtenion
    {
        public static IEnumerable<FileInfo> EnmuerateFiles(this DirectoryInfo dir, SubdirectoryType type)
        {
            if (type == SubdirectoryType.No) yield break;
            if (type == SubdirectoryType.This) type = SubdirectoryType.No;

            foreach (FileInfo file in dir.GetFiles()) yield return file;
            foreach (FileInfo file in EnumerateDirectories(dir, type).SelectMany(d => d.GetFiles()))
            {
                yield return file;
            }
        }

        public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo dir, SubdirectoryType type)
        {
            if (type == SubdirectoryType.No) yield break;

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                yield return subDir;

                if (type == SubdirectoryType.This) continue;

                foreach (DirectoryInfo subSubDir in EnumerateDirectories(subDir, SubdirectoryType.All))
                {
                    yield return subSubDir;
                }
            }
        }

        public static void OpenInExplorer(this DirectoryInfo dir)
        {
            Process.Start(dir.FullName);
        }

        public static void DeleteContent(this DirectoryInfo dir, SubdirectoryType type)
        {
            if (type == SubdirectoryType.No) return;

            if (type == SubdirectoryType.All)
            {
                foreach (DirectoryInfo subDir in EnumerateDirectories(dir, SubdirectoryType.This))
                {
                    DeleteContent(subDir, SubdirectoryType.All);
                    subDir.Delete(true);
                }
            }

            foreach (FileInfo file in EnmuerateFiles(dir, SubdirectoryType.This))
            {
                file.Delete();
            }
        }

        public static long GetLength(this DirectoryInfo dir, SubdirectoryType type)
        {
            return EnmuerateFiles(dir, type).Sum(f => f.Length);
        }
    }
}