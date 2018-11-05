using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FolderFile
{
    public enum SubfolderType { No, This, All }

    public class Folder
    {
        private FileInfo[] files;
        private Folder[] folders;

        private SubfolderType subfolder;

        public bool IsLoaded { get; private set; }

        public bool WithSubfolder
        {
            get { return subfolder == SubfolderType.All; }
            set { SubfolderType = value ? SubfolderType.All : SubfolderType.This; }
        }

        public string Path { get; private set; }

        public string FullPath { get; private set; }

        public DirectoryInfo Info { get; private set; }

        public SubfolderType SubfolderType
        {
            get { return subfolder; }
            set
            {
                if (value == subfolder) return;

                subfolder = value;

                RefreshFolderAndFiles();
            }
        }

        public Folder(string path, SubfolderType subfolderType)
        {
            Path = path;
            FullPath = GetFullPath(path);
            subfolder = subfolderType;

            Info = new DirectoryInfo(FullPath);

            RefreshFolderAndFiles();
        }

        private string GetFullPath(string path)
        {
            return path != "" ? System.IO.Path.GetFullPath(path) :
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public FileInfo[] GetFiles()
        {
            return GetFiles(subfolder == SubfolderType.All);
        }

        public FileInfo[] GetFiles(bool withSubfolder)
        {
            List<FileInfo> allFiles = new List<FileInfo>(files);

            if (withSubfolder)
            {
                foreach (Folder getFolder in folders)
                {
                    allFiles.AddRange(getFolder.GetFiles());
                }
            }

            return allFiles.ToArray();
        }

        public Folder[] GetFolders()
        {
            return GetFolders(subfolder == SubfolderType.All);
        }

        public Folder[] GetFolders(bool withSubfolder)
        {
            List<Folder> allFolder = new List<Folder>(folders);

            if (withSubfolder)
            {
                foreach (Folder getFolder in folders)
                {
                    allFolder.AddRange(getFolder.GetFolders());
                }
            }

            return allFolder.ToArray();
        }

        private FileInfo[] GetFilesFileArrayFromDirectory()
        {
            try
            {
                return Info.GetFiles();
            }
            catch (Exception e)
            {
                return new FileInfo[0];
            }
        }

        private Folder[] GetListFolderFromDirectory(SubfolderType subfolderType)
        {
            SubfolderType subfolderTypeOfSubfolder;

            if (subfolderType == SubfolderType.All) subfolderTypeOfSubfolder = SubfolderType.All;
            else subfolderTypeOfSubfolder = SubfolderType.No;

            try
            {
                return Info.GetDirectories().Select(d => new Folder(d.FullName, subfolderTypeOfSubfolder)).ToArray();
            }
            catch
            {
                return new Folder[0];
            }
        }

        public void OpenInExplorer()
        {
            Process.Start(FullPath);
        }

        public void RefreshFolderAndFiles()
        {
            RefreshFolderAndFiles(subfolder);
        }

        public void RefreshFolderAndFiles(SubfolderType subfolderType)
        {
            Info.Refresh();

            FileInfo[] newFiles = new FileInfo[0];
            Folder[] newFolders = new Folder[0];

            IsLoaded = false;
            subfolder = subfolderType;

            if (subfolderType != SubfolderType.No)
            {
                newFiles = GetFilesFileArrayFromDirectory();
                newFolders = GetListFolderFromDirectory(subfolderType);

                IsLoaded = true;
            }

            files = newFiles;
            folders = newFolders;
        }

        public void Delete()
        {
            DeleteContent();

            Info.Delete();
        }

        public void DeleteContent()
        {
            foreach (FileInfo file in GetFiles()) file.Delete();
            foreach (Folder folder in GetFolders()) folder.Delete();

            SubfolderType typeBackup = subfolder;
            RefreshFolderAndFiles();

            foreach (FileInfo file in GetFiles()) file.Delete();
            foreach (Folder folder in GetFolders()) folder.Delete();

            RefreshFolderAndFiles(typeBackup);
        }

        public void DeletContent(bool withSubfolder)
        {
            foreach (FileInfo file in GetFiles(withSubfolder)) file.Delete();

            SubfolderType typeBackup = subfolder;
            RefreshFolderAndFiles(withSubfolder ? SubfolderType.All : SubfolderType.This);

            foreach (FileInfo file in GetFiles(withSubfolder)) file.Delete();

            RefreshFolderAndFiles(typeBackup);
        }

        private long GetSize(bool withSubfolder)
        {
            long size = 0;
            FileInfo[] sizeOfFiles = GetFiles(withSubfolder);

            foreach (FileInfo file in sizeOfFiles)
            {
                size += file.Length;
            }

            return size;
        }

        public long GetRefreshedSize()
        {
            return GetRefreshedSize(WithSubfolder);
        }

        public long GetRefreshedSize(bool withSubfolder)
        {
            RefreshFolderAndFiles(withSubfolder ? SubfolderType.All : SubfolderType.This);

            return GetSize(withSubfolder);
        }

        public override string ToString()
        {
            return Path;
        }
    }
}