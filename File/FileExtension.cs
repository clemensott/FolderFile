using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderFileLib
{
    public static class FileExtension
    {
        public static string GetNameWithoutExtension(this FileInfo file)
        {
            return file.Name.Remove(file.Name.Length - file.Extension.Length);
        }
    }
}
