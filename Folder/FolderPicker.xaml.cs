using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System;
using System.Windows.Input;

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

        public static readonly DependencyProperty SubTypeSelectionProperty =
            DependencyProperty.Register("SubTypeSelection", typeof(SubfolderSelectionType), typeof(FolderPicker),
                new PropertyMetadata(SubfolderSelectionType.Auto, new PropertyChangedCallback(OnSubTypeSelectionPropertyChanged)));

        private static void OnSubTypeSelectionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (FolderPicker)sender;
            var oldValue = (SubfolderSelectionType)e.OldValue;
            var newValue = (SubfolderSelectionType)e.NewValue;

            if (s.Folder != null) s.UpdateCbxSubfolder();
            else
            {
                SubfolderType subType = GetSubfolderType(oldValue, s.cbxSubfolder.IsChecked);
                (bool isVisable, bool? isChecked) = GetCbxSubfolderProperties(newValue, subType);

                s.cbxSubfolder.Visibility = isVisable ? Visibility.Visible : Visibility.Collapsed;
                s.cbxSubfolder.IsChecked = isChecked;
            }
        }

        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(Folder), typeof(FolderPicker),
                new PropertyMetadata(null, new PropertyChangedCallback(OnFolderPropertyChanged)));

        private static void OnFolderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as FolderPicker;
            var oldValue = (Folder)e.OldValue;
            var newValue = (Folder)e.NewValue;

            if (oldValue != null) oldValue.PropertyChanged -= s.Folder_PropertyChanged;
            if (newValue != null) newValue.PropertyChanged -= s.Folder_PropertyChanged;

            if (newValue != null)
            {
                s.UpdateCbxSubfolder();

                s.tbxPath.Text = newValue.OriginalPath;
            }
            else
            {
                (bool isVisable, bool? isChecked) = GetCbxSubfolderProperties(s.SubTypeSelection, oldValue.SubType);

                s.cbxSubfolder.Visibility = isVisable ? Visibility.Visible : Visibility.Collapsed;
                s.cbxSubfolder.IsChecked = isChecked;
            }

            var args = new FolderChangedArgs(oldValue, newValue);
            s.FolderChanged?.Invoke(s, args);
        }

        public static readonly DependencyProperty IsChangeButtonVisableProperty =
            DependencyProperty.Register("IsChangeButtonVisable", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, new PropertyChangedCallback(OnIsChangeButtonVisablePropertyChanged)));

        private static void OnIsChangeButtonVisablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (FolderPicker)sender;
            var value = (bool)e.NewValue;

            s.btnChange.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty IsRefreshButtonVisableProperty =
            DependencyProperty.Register("IsRefreshButtonVisable", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, new PropertyChangedCallback(OnIsRefreshButtonVisablePropertyChanged)));

        private static void OnIsRefreshButtonVisablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (FolderPicker)sender;
            var value = (bool)e.NewValue;

            s.btnRefresh.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty IsOpenButtonVisableProperty =
            DependencyProperty.Register("IsOpenButtonVisable", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, new PropertyChangedCallback(OnIsOpenButtonVisablePropertyChanged)));

        private static void OnIsOpenButtonVisablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (FolderPicker)sender;
            var value = (bool)e.NewValue;

            s.btnOpen.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private readonly Brush tbxPathForeground;
        private System.Windows.Forms.FolderBrowserDialog fbd;

        public event EventHandler<FolderChangedArgs> FolderChanged;

        public bool SingleLine
        {
            get { return (bool)GetValue(SingleLineProperty); }
            set { SetValue(SingleLineProperty, value); }
        }

        public SubfolderSelectionType SubTypeSelection
        {
            get { return (SubfolderSelectionType)GetValue(SubTypeSelectionProperty); }
            set { SetValue(SubTypeSelectionProperty, value); }
        }

        public Folder Folder
        {
            get { return (Folder)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        public bool IsChangeButtonVisable
        {
            get { return (bool)GetValue(IsChangeButtonVisableProperty); }
            set { SetValue(IsChangeButtonVisableProperty, value); }
        }

        public bool IsRefreshButtonVisable
        {
            get { return (bool)GetValue(IsRefreshButtonVisableProperty); }
            set { SetValue(IsRefreshButtonVisableProperty, value); }
        }

        public bool IsOpenButtonVisable
        {
            get { return (bool)GetValue(IsOpenButtonVisableProperty); }
            set { SetValue(IsOpenButtonVisableProperty, value); }
        }

        public FolderPicker()
        {
            InitializeComponent();

            tbxPathForeground = tbxPath.Foreground;
            fbd = new System.Windows.Forms.FolderBrowserDialog();
        }

        private void TbxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Folder = new Folder(tbxPath.Text, GetSubfolderType());
                tbxPath.Foreground = tbxPathForeground;
            }
            catch
            {
                Folder = null;
                tbxPath.Foreground = errorBrush;
            }
        }

        private void CbxSubfolder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            cbxSubfolder.IsChecked = null;

            if (Folder == null) return;

            switch (SubTypeSelection)
            {
                case SubfolderSelectionType.Auto:
                case SubfolderSelectionType.Hidden:
                case SubfolderSelectionType.ThisAll:
                    Folder.SubType = SubfolderType.No;
                    break;

                case SubfolderSelectionType.NoThis:
                    Folder.SubType = SubfolderType.All;
                    break;

                case SubfolderSelectionType.NoAll:
                    Folder.SubType = SubfolderType.This;
                    break;
            }
        }

        private void CbxSubfolder_Checked(object sender, RoutedEventArgs e)
        {
            if (Folder == null) return;

            switch (SubTypeSelection)
            {
                case SubfolderSelectionType.Auto:
                case SubfolderSelectionType.Hidden:
                case SubfolderSelectionType.NoAll:
                case SubfolderSelectionType.ThisAll:
                    Folder.SubType = SubfolderType.All;
                    break;

                case SubfolderSelectionType.NoThis:
                    Folder.SubType = SubfolderType.This;
                    break;
            }
        }

        private void CbxSubfolder_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Folder == null) return;

            switch (SubTypeSelection)
            {
                case SubfolderSelectionType.Auto:
                case SubfolderSelectionType.Hidden:
                case SubfolderSelectionType.ThisAll:
                    Folder.SubType = SubfolderType.This;
                    break;

                case SubfolderSelectionType.NoThis:
                case SubfolderSelectionType.NoAll:
                    Folder.SubType = SubfolderType.No;
                    break;
            }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (Folder != null) fbd.SelectedPath = Folder.FullName;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Folder = new Folder(fbd.SelectedPath, GetSubfolderType());
                    tbxPath.Text = Folder.OriginalPath;
                }
                catch { }
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

            return GetSubfolderType(SubTypeSelection, cbxSubfolder.IsChecked);
        }

        private static SubfolderType GetSubfolderType(SubfolderSelectionType sel, bool? che)
        {
            if ((sel == SubfolderSelectionType.Auto && che == true) ||
                (sel == SubfolderSelectionType.Hidden && che == true) ||
                (sel == SubfolderSelectionType.NoAll && che == true) ||
                (sel == SubfolderSelectionType.NoThis && che == null) ||
                (sel == SubfolderSelectionType.ThisAll && che == true))
            {
                return SubfolderType.All;
            }
            else if ((sel == SubfolderSelectionType.Auto && che == false) ||
                (sel == SubfolderSelectionType.Hidden && che == false) ||
                (sel == SubfolderSelectionType.NoAll && che == null) ||
                (sel == SubfolderSelectionType.NoThis && che == true) ||
                (sel == SubfolderSelectionType.ThisAll && che == false))
            {
                return SubfolderType.This;
            }
            else return SubfolderType.No;
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
            if (Folder == null)
            {
                if (SubTypeSelection == SubfolderSelectionType.Hidden) cbxSubfolder.Visibility = Visibility.Collapsed;

                return;
            }

            (bool isVisable, bool? isChecked) = GetCbxSubfolderProperties(SubTypeSelection, Folder.SubType);

            cbxSubfolder.Visibility = isVisable ? Visibility.Visible : Visibility.Collapsed;
            cbxSubfolder.IsChecked = isChecked;
        }

        private static (bool isVisable, bool? isChecked) GetCbxSubfolderProperties(SubfolderSelectionType sel, SubfolderType sub)
        {
            bool isVisable;
            bool? isChecked;

            if ((sel == SubfolderSelectionType.Auto && sub == SubfolderType.No) ||
                (sel == SubfolderSelectionType.Hidden) ||
                (sel == SubfolderSelectionType.NoAll && sub == SubfolderType.This) ||
                (sel == SubfolderSelectionType.NoThis && sub == SubfolderType.All) ||
                (sel == SubfolderSelectionType.ThisAll && sub == SubfolderType.No))
            {
                isVisable = false;
            }
            else isVisable = true;


            if ((sel == SubfolderSelectionType.Auto && sub == SubfolderType.No) ||
                (sel == SubfolderSelectionType.Hidden && sub == SubfolderType.No) ||
                (sel == SubfolderSelectionType.NoAll && sub == SubfolderType.This) ||
                (sel == SubfolderSelectionType.NoThis && sub == SubfolderType.All) ||
                (sel == SubfolderSelectionType.ThisAll && sub == SubfolderType.No))
            {
                isChecked = null;
            }
            else if ((sel == SubfolderSelectionType.Auto && sub == SubfolderType.This) ||
                (sel == SubfolderSelectionType.Hidden && sub == SubfolderType.This) ||
                (sel == SubfolderSelectionType.NoAll && sub == SubfolderType.No) ||
                (sel == SubfolderSelectionType.NoThis && sub == SubfolderType.No) ||
                (sel == SubfolderSelectionType.ThisAll && sub == SubfolderType.This))
            {
                isChecked = false;
            }
            else isChecked = true;

            return (isVisable, isChecked);
        }
    }
}
