using System.ComponentModel;
using System.IO;
using System.Linq;

namespace FolderFile
{
    public class Folder : INotifyPropertyChanged
    {
        private SubfolderType subType;
        private FileInfo[] files;

        public string OriginalPath { get; private set; }

        public string Name { get; private set; }

        public string FullName { get; private set; }

        public DirectoryInfo Directory { get; private set; }

        public SubfolderType SubType
        {
            get { return subType; }
            set
            {
                if (value == subType) return;

                subType = value;
                OnPropertyChanged(nameof(SubType));
                Refresh();
            }
        }

        public FileInfo[] Files
        {
            get { return files; }
            set
            {
                if (value == files) return;

                files = value;
                OnPropertyChanged(nameof(Files));
            }
        }

        private Folder(SubfolderType subType, FileInfo[] files, string originalPath, string name, string fullName, DirectoryInfo directory)
        {
            this.subType = subType;
            this.files = files;
            OriginalPath = originalPath;
            Name = name;
            FullName = fullName;
            Directory = directory;
        }

        public Folder(string path, SubfolderType subType)
        {
            OriginalPath = path;
            SubType = subType;

            Directory = new DirectoryInfo(path);

            Name = Directory.Name;
            FullName = Directory.FullName;

            Refresh();
        }

        public Folder(DirectoryInfo directory, SubfolderType subType)
        {
            Directory = directory ?? throw new System.ArgumentNullException(nameof(directory));

            OriginalPath = Directory.FullName;
            Name = Directory?.Name;
            FullName = Directory?.FullName;

            Refresh();
        }

        public static bool TryCreate(SerializableFolder? serialFolder, out Folder folder)
        {
            try
            {
                if (serialFolder.HasValue)
                {
                    folder = serialFolder;
                    return true;
                }
                else
                {
                    folder = null;
                    return false;
                }
            }
            catch
            {
                folder = null;
                return false;
            }
        }

        public static bool TryCreate(string path, SubfolderType subType, out Folder folder)
        {
            try
            {
                folder = new Folder(path, subType);
                return true;
            }
            catch
            {
                folder = null;
                return false;
            }
        }

        public FileInfo[] Refresh()
        {
            if (Directory == null) return Files = new FileInfo[0];

            try
            {
                Directory.Refresh();
            }
            catch { }

            return Files = DirectoryInfoExtension.EnumerateFiles(Directory, SubType).ToArray();
        }

        public FileInfo[] RefreshThrow()
        {
            if (Directory == null) return Files = new FileInfo[0];

            Directory.Refresh();

            return Files = DirectoryInfoExtension.EnumerateFilesThrow(Directory, SubType).ToArray();
        }

        public void OpenInExplorer()
        {
            Directory.OpenInExplorer();
        }

        public void DeleteContent()
        {
            DeleteContent(SubType);
        }

        public void DeleteContent(SubfolderType type)
        {
            Directory?.DeleteContent(type);
        }

        public long GetLength()
        {
            return GetLength(SubType);
        }

        public long GetLength(SubfolderType type)
        {
            return Directory?.GetLength(type) ?? 0L;
        }

        public Folder Clone()
        {
            return new Folder(SubType, Files.ToArray(), OriginalPath, Name, FullName, new DirectoryInfo(Directory.FullName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
