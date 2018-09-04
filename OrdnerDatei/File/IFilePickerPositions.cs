using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FolderFile
{
    interface IFilePickerPositions
    {
        void SetPositionsAndMargin(TextBox tbxPath, Button btnChange, Button btnOpen);
    }
}
