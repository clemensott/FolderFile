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
    public partial class FilePicker : UserControl
    {
        private const double margin = 5;

        public static readonly DependencyProperty SingleLineProperty =
            DependencyProperty.Register("SingleLine", typeof(bool), typeof(DirectoryPicker),
                new PropertyMetadata(true, new PropertyChangedCallback(OnSingleLinePropertyChanged)));

        private static void OnSingleLinePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (DirectoryPicker)sender;
            var value = (bool)e.NewValue;

            if (value)
            {
                s.splMove.SetValue(Grid.ColumnProperty, 1);
                s.splMove.SetValue(Grid.RowProperty, 0);
                s.cdMargin.Width = new GridLength(margin);
            }
            else
            {
                s.splMove.SetValue(Grid.ColumnProperty, 0);
                s.splMove.SetValue(Grid.RowProperty, 1);
                s.cdMargin.Width = new GridLength(0);
            }
        }

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(FileInfo), typeof(FilePicker),
                new PropertyMetadata(null, new PropertyChangedCallback(OnFilePropertyChanged)));

        protected static void OnFilePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FilePicker;
            var value = (FileInfo)e.NewValue;

            s.tbxPath.Text = s.fd.FileName = value?.FullName;
        }

        protected FileDialog fd;

        public bool SingleLine
        {
            get { return (bool)GetValue(SingleLineProperty); }
            set { SetValue(SingleLineProperty, value); }
        }

        public FileInfo File
        {
            get { return (FileInfo)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public FilePicker()
        {
            InitializeComponent();

            SingleLine = true;
        }

        private void TbxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            File = System.IO.File.Exists(tbxPath.Text) ? new FileInfo(tbxPath.Text) : null;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (fd.ShowDialog() == true)
            {
                File = new FileInfo(fd.FileName);
                tbxPath.Text = fd.FileName;
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(tbxPath.Text);
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            try
            {
                tbxPath.Text = e.Data.GetData(DataFormats.Text).ToString();
            }
            catch { }
        }
    }
}
