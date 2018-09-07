using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FolderFile
{
    interface IFolderPickerPositions
    {
        void SetPositionsAndMargin(TextBox tbxPath, CheckBox cbxSubfolder, Button btnChange, Button btnOpen);
    }
}
