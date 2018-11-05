using System.Windows.Controls;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media;

namespace FolderFile
{
    /// <summary>
    /// Interaktionslogik für DirectoryPicker.xaml
    /// </summary>
    public partial class DirectoryPicker : UserControl
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

        public static readonly DependencyProperty SubdirectoryTypeProperty =
            DependencyProperty.Register("SubdirectoryType", typeof(SubdirectoryType), typeof(DirectoryPicker),
                new PropertyMetadata(SubdirectoryType.No, new PropertyChangedCallback(OnSubdirectoryTypePropertyChanged)));

        private static void OnSubdirectoryTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (DirectoryPicker)sender;
            var value = (SubdirectoryType)e.NewValue;

            s.CbxSubfolder.Visibility = value == SubdirectoryType.No ? Visibility.Collapsed : Visibility.Visible;

            s.Files = s.Directory?.EnmuerateFiles(value);
        }

        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(IEnumerable<FileInfo>), typeof(DirectoryPicker),
                new PropertyMetadata(null, new PropertyChangedCallback(OnFilesPropertyChanged)));

        private static void OnFilesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (DirectoryPicker)sender;
            var value = (IEnumerable<FileInfo>)e.NewValue;
        }

        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.Register("Directory",
            typeof(DirectoryInfo), typeof(DirectoryPicker), new PropertyMetadata(
                new PropertyChangedCallback(OnDirectoryPropertyChanged)));

        private static void OnDirectoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as DirectoryPicker;
            var value = (DirectoryInfo)e.NewValue;

            s.Files = value?.EnmuerateFiles(s.SubdirectoryType);
        }

        private System.Windows.Forms.FolderBrowserDialog fbd;

        public bool SingleLine
        {
            get { return (bool)GetValue(SingleLineProperty); }
            set { SetValue(SingleLineProperty, value); }
        }

        public SubdirectoryType SubdirectoryType
        {
            get { return (SubdirectoryType)GetValue(SubdirectoryTypeProperty); }
            set { SetValue(SubdirectoryTypeProperty, value); }
        }

        public IEnumerable<FileInfo> Files
        {
            get { return (IEnumerable<FileInfo>)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        public DirectoryInfo Directory
        {
            get { return (DirectoryInfo)GetValue(DirectoryProperty); }
            set { SetValue(DirectoryProperty, value); }
        }

        public DirectoryPicker()
        {
            InitializeComponent();

            fbd = new System.Windows.Forms.FolderBrowserDialog();
        }

        private void TbxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.IO.Directory.Exists(tbxPath.Text))
            {
                tbxPath.BorderBrush = new SolidColorBrush(Colors.Transparent);
                Directory = new DirectoryInfo(tbxPath.Text);
            }
            else
            {
                tbxPath.BorderBrush = new SolidColorBrush(Colors.Red);
                Directory = null;
            }
        }

        private void CbxSubfolder_Checked(object sender, RoutedEventArgs e)
        {
            SubdirectoryType = SubdirectoryType.All;
        }

        private void CbxSubfolder_Unchecked(object sender, RoutedEventArgs e)
        {
            SubdirectoryType = SubdirectoryType.This;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directory = new DirectoryInfo(fbd.SelectedPath);
                tbxPath.Text = Directory.FullName;
            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            Files = Directory?.EnmuerateFiles(SubdirectoryType);
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory?.OpenInExplorer();
            }
            catch { }
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
