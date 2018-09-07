using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FolderFile
{
    /// <summary>
    /// Interaktionslogik für FilePicker.xaml
    /// </summary>
    public partial class FilePicker : UserControl, INotifyPropertyChanged
    {
        protected bool twoLines;
        protected FileDialog fd;
        private IFilePickerPositions positionSeter;

        public static readonly DependencyProperty FileProperty = 
            DependencyProperty.Register("File", typeof(FileInfo), typeof(FilePicker),
                new PropertyMetadata(new FileInfo(""), new PropertyChangedCallback(OnFirstFilePropertyChanged)));

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileSavePicker),
            new PropertyMetadata("Alle Dateien|*.*", new PropertyChangedCallback(OnFirstFilterPropertyChanged)));

        protected static void OnFirstFilePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FilePicker;

            if (s != null)
            {
                s.File = e.NewValue as FileInfo;
            }
        }

        protected static void OnFirstFilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FilePicker;

            if (s != null)
            {
                s.Filter = e.NewValue as string;
            }

        }

        public FileInfo File
        {
            get { return (FileInfo)GetValue(FileProperty); }
            set
            {
                SetValue(FileProperty, value);

                if (value != null)
                {
                    tbx_path.Text = fd.FileName = value.FullName;
                }
            }
        }

        public string Filter
        {
            get { return (string)GetValue(FileProperty); }
            set
            {
                SetValue(FilterProperty, value);

                if (value != null)
                {
                    fd.Filter = value;
                }
            }
        }

        public bool TwoLines
        {
            get { return twoLines; }
            set
            {
                if (twoLines == value) return;

                twoLines = value;

                if (twoLines) positionSeter = new FilePickerPositionsTwoLines();
                else positionSeter = new FilePickerPositionsOneLine();

                positionSeter.SetPositionsAndMargin(tbx_path, btn_change, btn_open);
            }
        }

        public FilePicker()
        {
            InitializeComponent();

            TwoLines = false;
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() == true)
            {
                File = new FileInfo(fd.FileName);
                tbx_path.Text = fd.FileName;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(tbx_path.Text);
        }

        private void tbx_path_LostFocus(object sender, RoutedEventArgs e)
        {
            FileInfo file = new FileInfo(tbx_path.Text);

            //if (file.Exists() || (file.FullPath != "" && MessageBox.Show("Die File existiert nicht.\n\nTrotzdem übernehmen?",
            //    "Fehler", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            //{
            File = file;
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string dropText = e.Data.GetData(DataFormats.Text).ToString();

                File = new FileInfo(dropText);
            }
            catch { }
        }
    }
}
