using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FolderFile
{
    /// <summary>
    /// Interaktionslogik für FolderPicker.xaml
    /// </summary>
    public partial class FolderPicker : UserControl
    {
        private bool withCheckbox, twoLines;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private IFolderPickerPositions positionSeter;

        public static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder",
            typeof(Folder), typeof(FolderPicker), new PropertyMetadata(new Folder("", SubfolderType.No),
                new PropertyChangedCallback(OnFolderPropertyChanged)));

        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.Register("Directory",
            typeof(DirectoryInfo), typeof(FolderPicker), new PropertyMetadata(
                new PropertyChangedCallback(OnDirectoryPropertyChanged)));

        private static void OnFolderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FolderPicker;
            var value = (Folder)e.NewValue;

            s.cbx_uo.IsChecked = value?.WithSubfolder ?? false;
            s.tbx_path.Text = s.fbd.SelectedPath = value?.Path;

            if (s.Directory == null || value?.Info.FullName != s.Directory.FullName ||
                value?.Info.LastWriteTime != s.Directory.LastWriteTime) s.Directory = value?.Info;
        }

        private static void OnDirectoryPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FolderPicker;
            var value = (DirectoryInfo)e.NewValue;

            if (s.Folder?.Info == null || s.Folder?.Info?.FullName != value.FullName ||
                s.Folder.Info.LastWriteTime != value.LastWriteTime) s.Folder = s.GetFolder(value.FullName);
        }

        public DirectoryInfo Directory
        {
            get { return (DirectoryInfo)GetValue(DirectoryProperty); }
            set { SetValue(DirectoryProperty, value); }
        }

        public Folder Folder
        {
            get { return (Folder)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        public bool WithCheckbox
        {
            get { return withCheckbox; }
            set
            {
                if (withCheckbox == value) return;

                withCheckbox = value;
                cbx_uo.Visibility = withCheckbox ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool TwoLines
        {
            get { return twoLines; }
            set
            {
                if (twoLines == value) return;

                twoLines = value;

                if (twoLines) positionSeter = new FolderPickerPositionsTwoLines();
                else positionSeter = new FolderPickerPositionsOneLine();

                positionSeter.SetPositionsAndMargin(tbx_path, cbx_uo, btn_change, btn_open);
            }
        }

        public FolderPicker()
        {
            InitializeComponent();

            fbd = new System.Windows.Forms.FolderBrowserDialog();
            withCheckbox = !withCheckbox;
            WithCheckbox = !withCheckbox;
            TwoLines = twoLines;
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder = GetFolder(fbd.SelectedPath);
                tbx_path.Text = Folder.Path;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Folder?.OpenInExplorer();
            }
            catch { }
        }

        private void tbx_path_LostFocus(object sender, RoutedEventArgs e)
        {
            Folder directory = GetFolder(tbx_path.Text);

            //if (directory.Exists() || (directory.FullPath != "" && MessageBox.Show("Der Folder \"" +
            //    directory.FullPath + "\" existiert nicht.\n\nTrotzdem übernehmen?",
            //    "Fehler", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            //{
            Folder = GetFolder(tbx_path.Text);
            //}
        }

        private void cbx_uo_Checked(object sender, RoutedEventArgs e)
        {
            if (Folder.WithSubfolder != cbx_uo.IsChecked)
            {
                Folder = GetFolder(tbx_path.Text);
            }
        }

        private void tbx_path_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.IO.Directory.Exists(tbx_path.Text)) Folder = GetFolder(tbx_path.Text);
        }

        private void Control_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string dropText = e.Data.GetData(DataFormats.Text).ToString();

                Folder = GetFolder(dropText);
            }
            catch { }
        }

        private Folder GetFolder(string path)
        {
            return new Folder(path, GetSubfolderType());
        }

        private SubfolderType GetSubfolderType()
        {
            return cbx_uo.IsChecked == true ? SubfolderType.All : SubfolderType.This;
        }
    }
}
