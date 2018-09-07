using Microsoft.Win32;

namespace FolderFile
{
    public class FileSavePicker : FilePicker
    {
        public FileSavePicker()
        {
            fd = new SaveFileDialog();
        }
    }
}
