using System.Windows.Controls;

namespace FolderFile
{
    interface IFolderPickerPositions
    {
        void SetPositionsAndMargin(TextBox tbxPath, CheckBox cbxSubfolder, Button btnChange, Button btnOpen);
    }
}
