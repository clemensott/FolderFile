using System;

namespace FolderFile
{
    public class FolderChangedArgs : EventArgs
    {
        public Folder OldFolder { get; private set; }

        public Folder NewFolder { get; private set; }

        public FolderChangedArgs(Folder oldFolder, Folder newFolder)
        {
            OldFolder = oldFolder;
            NewFolder = newFolder;
        }
    }
}
