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

        public Folder(string path, SubfolderType subType)
        {
            OriginalPath = path;
            SubType = subType;

            try
            {
                Directory = new DirectoryInfo(path);
            }
            catch { }

            Name = Directory?.Name;
            FullName = Directory?.FullName;

            Refresh();
        }

        public Folder(DirectoryInfo directory, SubfolderType subType)
        {
            Directory = directory;

            OriginalPath = Directory?.FullName;
            Name = Directory?.Name;
            FullName = Directory?.FullName;

            Refresh();
        }

        public FileInfo[] Refresh()
        {
            if (Directory == null) return Files = new FileInfo[0];

            try
            {
                return Files = DirectoryInfoExtension.EnumerateFiles(Directory, SubType).ToArray();
            }
            catch
            {
                return Files = new FileInfo[0];
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
