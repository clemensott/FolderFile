using System;

namespace FolderFile
{
    public class FolderChangedArgs : EventArgs
    {
        public Folder OldFolder { get; }

        public Folder NewFolder { get; }

        public FolderChangedArgs(Folder oldFolder, Folder newFolder)
        {
            OldFolder = oldFolder;
            NewFolder = newFolder;
        }
    }
}
