using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FolderFile
{
    class FilePickerPositionsOneLine : IFilePickerPositions
    {
        public void SetPositionsAndMargin(TextBox tbxPath, Button btnChange, Button btnOpen)
        {
            btnChange.Margin = new Thickness(0, 6, 0, 0);
            btnOpen.Margin = new Thickness(0);
            tbxPath.Margin = new Thickness(0);

            btnChange.SetValue(Grid.RowProperty, 0);
            btnOpen.SetValue(Grid.RowProperty, 0);
            tbxPath.SetValue(Grid.ColumnSpanProperty, 1);
        }
    }
}
