using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FolderFile
{
    public class Folder : INotifyPropertyChanged
    {
        public const bool AutoRefreshDefault = true;
        public const SubfolderType SubTypeDefault = SubfolderType.No;

        private bool autoRefresh, exists;
        private SubfolderType subType;
        private FileInfo[] files;
        private Task<FileInfo[]> refreshTask;

        public bool AutoRefresh
        {
            get => autoRefresh;
            set
            {
                if (value == autoRefresh) return;

                autoRefresh = value;
                OnPropertyChanged(nameof(AutoRefresh));
            }
        }

        public bool Exists
        {
            get => exists;
            private set
            {
                if (value == exists) return;

                exists = value;
                OnPropertyChanged(nameof(Exists));
            }
        }

        public string OriginalPath { get; }

        public string Name { get; }

        public string FullName { get; }

        public SubfolderType SubType
        {
            get => subType;
            set
            {
                if (value == subType) return;

                subType = value;
                OnPropertyChanged(nameof(SubType));

                if (AutoRefresh) RefreshAsync();
            }
        }

        public FileInfo[] Files
        {
            get => files;
            set
            {
                if (value == files) return;

                files = value;
                OnPropertyChanged(nameof(Files));
            }
        }

        public Task<FileInfo[]> RefreshTask
        {
            get => refreshTask;
            set
            {
                if (value == refreshTask) return;

                refreshTask = value;
                OnPropertyChanged(nameof(RefreshTask));
            }
        }

        private Folder(SubfolderType subType, FileInfo[] files, string originalPath,
            string name, string fullName, bool autoRefresh, bool exists)
        {
            this.subType = subType;
            this.files = files;
            OriginalPath = originalPath;
            Name = name;
            FullName = fullName;
            this.autoRefresh = autoRefresh;
            this.exists = exists;
        }

        public Folder(string path) : this(path, SubTypeDefault, AutoRefreshDefault)
        {
        }

        public Folder(string path, SubfolderType subType) : this(path, subType, AutoRefreshDefault)
        {
        }

        public Folder(string path, bool autoRefresh) : this(path, SubTypeDefault, autoRefresh)
        {
        }

        public Folder(string path, SubfolderType subType, bool autoRefresh)
        {
            AutoRefresh = autoRefresh;
            OriginalPath = path;

            Name = Path.GetFileName(OriginalPath);
            FullName = Path.GetFullPath(OriginalPath);
            Exists = Directory.Exists(FullName);

            Files = new FileInfo[0];
            SubType = subType;
        }

        public FileInfo[] Refresh()
        {
            try
            {
                Exists = Directory.Exists(FullName);

                return Files = GetDirectory().EnumerateFiles(SubType).ToArray();
            }
            catch
            {
                return Files = new FileInfo[0];
            }
        }

        public async Task<FileInfo[]> RefreshAsync()
        {
            Task<FileInfo[]> task = RefreshTask = Task.Run(() => GetDirectory().EnumerateFiles(SubType).ToArray());

            try
            {
                Exists = Directory.Exists(FullName);

                FileInfo[] refreshFiles = await task;
                if (task == RefreshTask || (!RefreshTask.IsCompleted && !RefreshTask.IsCanceled && !RefreshTask.IsFaulted))
                {
                    Files = refreshFiles;
                }

                return refreshFiles;
            }
            catch
            {
                FileInfo[] refreshFiles = new FileInfo[0];
                if (task == RefreshTask || (!RefreshTask.IsCompleted && !RefreshTask.IsCanceled && !RefreshTask.IsFaulted))
                {
                    Files = refreshFiles;
                }

                return refreshFiles;
            }
        }

        public FileInfo[] RefreshThrow()
        {
            Exists = Directory.Exists(FullName);

            return Files = GetDirectory().EnumerateFilesThrow(SubType).ToArray();
        }

        public async Task<FileInfo[]> RefreshThrowAsync()
        {
            Task<FileInfo[]> task = RefreshTask = Task.Run(() => GetDirectory().EnumerateFilesThrow(SubType).ToArray());
            Exists = Directory.Exists(FullName);

            FileInfo[] refreshFiles = await task;
            if (task == RefreshTask || (!RefreshTask.IsCompleted && !RefreshTask.IsCanceled && !RefreshTask.IsFaulted))
            {
                Files = refreshFiles;
            }

            return refreshFiles;
        }

        public DirectoryInfo GetDirectory() => new DirectoryInfo(OriginalPath);

        public void OpenInExplorer()
        {
            string args = string.Format("/select,\"{0}\"", FullName);
            Process.Start("explorer.exe", args);
        }

        public void DeleteContent()
        {
            DeleteContent(SubType);
        }

        public void DeleteContent(SubfolderType type)
        {
            GetDirectory().DeleteContent(type);
        }

        public long GetLength()
        {
            return GetLength(SubType);
        }

        public long GetLength(SubfolderType type)
        {
            return GetDirectory().GetLength(type);
        }

        public Folder Clone()
        {
            return new Folder(SubType, Files?.ToArray(), OriginalPath, Name, FullName, AutoRefresh, Exists);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static Folder CreateOrDefault(SerializableFolder? serialFolder)
        {
            Folder folder;
            return TryCreate(serialFolder, out folder) ? folder : null;
        }

        public static bool TryCreate(SerializableFolder? serialFolder, out Folder folder)
        {
            try
            {
                if (serialFolder.HasValue)
                {
                    folder = (Folder)serialFolder;
                    return true;
                }

                folder = null;
                return false;
            }
            catch
            {
                folder = null;
                return false;
            }
        }

        public static Folder CreateOrDefault(string path)
        {
            Folder folder;
            return TryCreate(path, out folder) ? folder : null;
        }

        public static bool TryCreate(string path, out Folder folder)
        {
            return TryCreate(path, SubTypeDefault, AutoRefreshDefault, out folder);
        }

        public static Folder CreateOrDefault(string path, SubfolderType subType)
        {
            Folder folder;
            return TryCreate(path, subType, out folder) ? folder : null;
        }

        public static bool TryCreate(string path, SubfolderType subType, out Folder folder)
        {
            return TryCreate(path, subType, AutoRefreshDefault, out folder);
        }

        public static Folder CreateOrDefault(string path, SubfolderType subType, bool autoRefresh)
        {
            Folder folder;
            return TryCreate(path, subType, autoRefresh, out folder) ? folder : null;
        }

        public static bool TryCreate(string path, SubfolderType subType, bool autoRefresh, out Folder folder)
        {
            try
            {
                folder = new Folder(path, subType, autoRefresh);
                return true;
            }
            catch
            {
                folder = null;
                return false;
            }
        }
    }
}
