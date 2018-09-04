using Microsoft.Win32;

namespace FolderFile
{
    public class FileOpenPicker : FilePicker
    {
        public FileOpenPicker()
        {
            fd = new OpenFileDialog();
        }
    }
}
