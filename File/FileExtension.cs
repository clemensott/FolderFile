using System.IO;

namespace FolderFile
{
    public static class FileExtension
    {
        public static string GetNameWithoutExtension(this FileInfo file)
        {
            return file.Name.Remove(file.Name.Length - file.Extension.Length);
        }
    }
}
