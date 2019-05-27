using Microsoft.Win32;
using System.Windows;

namespace FolderFile
{
    public class FileSavePicker : FilePicker
    {
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileSavePicker),
            new PropertyMetadata("All files|*.*", new PropertyChangedCallback(OnFilterPropertyChanged)));

        private static void OnFilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileSavePicker s = (FileSavePicker)sender;
            string value = (string)e.NewValue;

            s.fd.Filter = value;
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public FileSavePicker()
        {
            fd = new SaveFileDialog();

            fd.Filter = Filter;
        }
    }
}
