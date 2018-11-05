using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media;

namespace FolderFile
{
    public class FileOpenPicker : FilePicker
    {
        public FileOpenPicker()
        {
            fd = new OpenFileDialog();

            tbxPath.TextChanged += TbxPath_TextChanged;
        }

        private void TbxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbxPath.BorderBrush = File != null ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.Red);
        }
    }
}
