using System.Windows.Controls;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.ComponentModel;

namespace FolderFile
{
    /// <summary>
    /// Interaktionslogik für FolderPicker.xaml
    /// </summary>
    public partial class FolderPicker : UserControl
    {
        private const double margin = 5;
        private static readonly Brush errorBrush = new SolidColorBrush(Colors.Red);

        public static readonly DependencyProperty SingleLineProperty =
            DependencyProperty.Register("SingleLine", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, new PropertyChangedCallback(OnSingleLinePropertyChanged)));

        private static void OnSingleLinePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (FolderPicker)sender;
            var value = (bool)e.NewValue;

            if (value)
            {
                s.splMove.SetValue(Grid.ColumnProperty, 2);
                s.splMove.SetValue(Grid.RowProperty, 0);
                s.cdMargin.Width = new GridLength(margin);
                s.rdMargin.Height = new GridLength(0);
            }
            else
            {
                s.splMove.SetValue(Grid.ColumnProperty, 0);
                s.splMove.SetValue(Grid.RowProperty, 2);
                s.cdMargin.Width = new GridLength(0);
                s.rdMargin.Height = new GridLength(margin);
            }
        }

        public static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder",
            typeof(Folder), typeof(FolderPicker), new PropertyMetadata(
                new PropertyChangedCallback(OnFolderPropertyChanged)));

        private static void OnFolderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FolderPicker;
            var oldValue = (Folder)e.OldValue;
            var newValue = (Folder)e.NewValue;

            if (oldValue != null) oldValue.PropertyChanged -= s.Folder_PropertyChanged;
            if (newValue != null) newValue.PropertyChanged -= s.Folder_PropertyChanged;

            s.UpdateCbxSubfolder();
            s.tbxPath.Text = newValue?.OriginalPath ?? string.Empty;
        }

        private readonly Brush tbxPathForeground;
        private System.Windows.Forms.FolderBrowserDialog fbd;

        public bool SingleLine
        {
            get { return (bool)GetValue(SingleLineProperty); }
            set { SetValue(SingleLineProperty, value); }
        }

        public Folder Folder
        {
            get { return (Folder)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        public FolderPicker()
        {
            InitializeComponent();

            tbxPathForeground = tbxPath.Foreground;
            fbd = new System.Windows.Forms.FolderBrowserDialog();
        }

        private void TbxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbxPath.Foreground = Directory.Exists(tbxPath.Text) ? tbxPathForeground : errorBrush;

            Folder = new Folder(tbxPath.Text, GetSubfolderType());
        }

        private void CbxSubfolder_Checked(object sender, RoutedEventArgs e)
        {
            if (Folder != null) Folder.SubType = SubfolderType.All;
        }

        private void CbxSubfolder_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Folder != null) Folder.SubType = SubfolderType.This;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (Folder != null) fbd.SelectedPath = Folder.FullName;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder = new Folder(fbd.SelectedPath, GetSubfolderType());
                tbxPath.Text = Folder.OriginalPath;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Folder?.Refresh();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Folder?.OpenInExplorer();
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

        private SubfolderType GetSubfolderType()
        {
            if (Folder != null) return Folder.SubType;

            switch (cbxSubfolder.IsChecked)
            {
                case true:
                    return SubfolderType.All;

                case false:
                    return SubfolderType.This;

                case null:
                default:
                    return SubfolderType.No;
            }
        }

        private void Folder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Folder.SubType):
                    UpdateCbxSubfolder();
                    break;
            }
        }

        private void UpdateCbxSubfolder()
        {
            if (Folder == null) return;

            switch (Folder.SubType)
            {
                case SubfolderType.No:
                    cbxSubfolder.Visibility = Visibility.Collapsed;
                    cbxSubfolder.IsChecked = null;
                    break;

                case SubfolderType.This:
                    cbxSubfolder.Visibility = Visibility.Visible;
                    cbxSubfolder.IsChecked = false;
                    break;

                case SubfolderType.All:
                    cbxSubfolder.Visibility = Visibility.Visible;
                    cbxSubfolder.IsChecked = true;
                    break;
            }
        }
    }
}
