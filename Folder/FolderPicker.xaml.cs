using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System;
using System.Windows.Input;

namespace FolderFile
{
    public partial class FolderPicker : UserControl
    {
        private const double margin = 5;
        private static readonly Brush errorBrush = new SolidColorBrush(Colors.Red);

        public static readonly DependencyProperty SingleLineProperty =
            DependencyProperty.Register("SingleLine", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, OnSingleLinePropertyChanged));

        private static void OnSingleLinePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            bool value = (bool)e.NewValue;

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

        public static readonly DependencyProperty SubTypeSelectionProperty = DependencyProperty.Register("SubTypeSelection",
            typeof(SubfolderSelectionType), typeof(FolderPicker),
                new PropertyMetadata(SubfolderSelectionType.Auto, OnSubTypeSelectionPropertyChanged));

        private static void OnSubTypeSelectionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            SubfolderSelectionType oldValue = (SubfolderSelectionType)e.OldValue;
            SubfolderSelectionType newValue = (SubfolderSelectionType)e.NewValue;

            if (s.Folder != null) s.UpdateCbxSubfolder();
            else
            {
                SubfolderType subType = GetSubfolderType(oldValue, s.cbxSubfolder.IsChecked);
                (bool isVisible, bool? isChecked) = GetCbxSubfolderProperties(newValue, subType);

                s.cbxSubfolder.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                s.cbxSubfolder.IsChecked = isChecked;
            }
        }

        public static readonly DependencyProperty AutoRefreshSelectionProperty = DependencyProperty.Register("AutoRefreshSelection",
            typeof(AutoRefreshSelectionType), typeof(FolderPicker),
                new PropertyMetadata(AutoRefreshSelectionType.Visible, OnAutoRefreshSelectionPropertyChanged));

        private static void OnAutoRefreshSelectionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            AutoRefreshSelectionType value = (AutoRefreshSelectionType)e.NewValue;

            s.cbxAutoRefresh.Visibility = value == AutoRefreshSelectionType.Visible ? Visibility.Visible : Visibility.Collapsed;

            switch (value)
            {
                case AutoRefreshSelectionType.True:
                case AutoRefreshSelectionType.ForceTrue:
                    s.cbxAutoRefresh.IsChecked = true;
                    break;

                case AutoRefreshSelectionType.False:
                case AutoRefreshSelectionType.ForceFalse:
                    s.cbxAutoRefresh.IsChecked = false;
                    break;
            }

            if (s.Folder == null) return;

            switch (value)
            {
                case AutoRefreshSelectionType.Visible:
                    s.cbxAutoRefresh.IsChecked = s.Folder.AutoRefresh;
                    break;

                case AutoRefreshSelectionType.Collapsed:
                    s.cbxAutoRefresh.IsChecked = s.Folder.AutoRefresh;
                    break;

                case AutoRefreshSelectionType.ForceTrue:
                    s.Folder.AutoRefresh = true;
                    break;

                case AutoRefreshSelectionType.ForceFalse:
                    s.Folder.AutoRefresh = false;
                    break;
            }
        }

        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(Folder), typeof(FolderPicker),
                new PropertyMetadata(null, OnFolderPropertyChanged));

        private static void OnFolderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            Folder oldValue = (Folder)e.OldValue;
            Folder newValue = (Folder)e.NewValue;

            if (oldValue != null) oldValue.PropertyChanged -= s.Folder_PropertyChanged;
            if (newValue != null) newValue.PropertyChanged -= s.Folder_PropertyChanged;

            if (newValue != null)
            {
                s.UpdateCbxSubfolder();

                s.tbxPath.Text = newValue.OriginalPath;
                s.tbxPath.Foreground = newValue.Exists ? s.tbxPathForeground : errorBrush;

                switch (s.AutoRefreshSelection)
                {
                    case AutoRefreshSelectionType.Visible:
                        s.cbxAutoRefresh.IsChecked = newValue.AutoRefresh;
                        break;

                    case AutoRefreshSelectionType.Collapsed:
                        s.cbxAutoRefresh.IsChecked = newValue.AutoRefresh;
                        break;

                    case AutoRefreshSelectionType.ForceTrue:
                        newValue.AutoRefresh = true;
                        break;

                    case AutoRefreshSelectionType.ForceFalse:
                        newValue.AutoRefresh = false;
                        break;
                }
            }
            else
            {
                (bool isVisible, bool? isChecked) = GetCbxSubfolderProperties(s.SubTypeSelection, oldValue.SubType);

                s.cbxSubfolder.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                s.cbxSubfolder.IsChecked = isChecked;
                s.tbxPath.Foreground = errorBrush;
                s.tbxPath.Text = string.Empty;
            }

            FolderChangedArgs args = new FolderChangedArgs(oldValue, newValue);
            s.FolderChanged?.Invoke(s, args);
        }

        public static readonly DependencyProperty IsChangeButtonVisibleProperty =
            DependencyProperty.Register("IsChangeButtonVisible", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, OnIsChangeButtonVisiblePropertyChanged));

        private static void OnIsChangeButtonVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            bool value = (bool)e.NewValue;

            s.btnChange.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty IsRefreshButtonVisibleProperty =
            DependencyProperty.Register("IsRefreshButtonVisible", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, OnIsRefreshButtonVisiblePropertyChanged));

        private static void OnIsRefreshButtonVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            bool value = (bool)e.NewValue;

            s.btnRefresh.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty IsOpenButtonVisibleProperty =
            DependencyProperty.Register("IsOpenButtonVisible", typeof(bool), typeof(FolderPicker),
                new PropertyMetadata(true, OnIsOpenButtonVisiblePropertyChanged));

        private static void OnIsOpenButtonVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderPicker s = (FolderPicker)sender;
            bool value = (bool)e.NewValue;

            s.btnOpen.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private readonly Brush tbxPathForeground;
        private System.Windows.Forms.FolderBrowserDialog fbd;

        public event EventHandler<FolderChangedArgs> FolderChanged;

        public bool SingleLine
        {
            get => (bool)GetValue(SingleLineProperty);
            set => SetValue(SingleLineProperty, value);
        }

        public SubfolderSelectionType SubTypeSelection
        {
            get => (SubfolderSelectionType)GetValue(SubTypeSelectionProperty);
            set => SetValue(SubTypeSelectionProperty, value);
        }

        public AutoRefreshSelectionType AutoRefreshSelection
        {
            get => (AutoRefreshSelectionType)GetValue(AutoRefreshSelectionProperty);
            set => SetValue(AutoRefreshSelectionProperty, value);
        }

        public Folder Folder
        {
            get => (Folder)GetValue(FolderProperty);
            set => SetValue(FolderProperty, value);
        }

        public bool IsChangeButtonVisible
        {
            get => (bool)GetValue(IsChangeButtonVisibleProperty);
            set => SetValue(IsChangeButtonVisibleProperty, value);
        }

        public bool IsRefreshButtonVisible
        {
            get => (bool)GetValue(IsRefreshButtonVisibleProperty);
            set => SetValue(IsRefreshButtonVisibleProperty, value);
        }

        public bool IsOpenButtonVisible
        {
            get => (bool)GetValue(IsOpenButtonVisibleProperty);
            set => SetValue(IsOpenButtonVisibleProperty, value);
        }

        public FolderPicker()
        {
            InitializeComponent();

            tbxPathForeground = tbxPath.Foreground;
            fbd = new System.Windows.Forms.FolderBrowserDialog();
        }

        private void TbxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Folder?.OriginalPath == tbxPath.Text) return;

            Folder folder;
            if (Folder.TryCreate(tbxPath.Text, GetSubfolderType(), cbxAutoRefresh.IsChecked == true, out folder))
            {
                Folder = folder;
            }
            else
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

        private void CbxAutoRefresh_Checked(object sender, RoutedEventArgs e)
        {
            if (Folder != null) Folder.AutoRefresh = true;
        }

        private void CbxAutoRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Folder != null) Folder.AutoRefresh = false;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (Folder != null) fbd.SelectedPath = Folder.FullName;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Folder folder;
                if (Folder.TryCreate(fbd.SelectedPath, GetSubfolderType(), cbxAutoRefresh.IsChecked == true, out folder))
                {
                    Folder = folder;
                    tbxPath.Text = Folder.OriginalPath;
                }
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

                case nameof(Folder.Exists):
                    tbxPath.Foreground = Folder.Exists ? tbxPathForeground : errorBrush;
                    break;

                case nameof(Folder.AutoRefresh) when AutoRefreshSelection == AutoRefreshSelectionType.Visible:
                case nameof(Folder.AutoRefresh) when AutoRefreshSelection == AutoRefreshSelectionType.Collapsed:
                    cbxAutoRefresh.IsChecked = Folder.AutoRefresh;
                    break;

                case nameof(Folder.AutoRefresh) when AutoRefreshSelection == AutoRefreshSelectionType.ForceTrue:
                    Folder.AutoRefresh = true;
                    break;

                case nameof(Folder.AutoRefresh) when AutoRefreshSelection == AutoRefreshSelectionType.ForceFalse:
                    Folder.AutoRefresh = false;
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

            (bool isVisible, bool? isChecked) = GetCbxSubfolderProperties(SubTypeSelection, Folder.SubType);

            cbxSubfolder.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            cbxSubfolder.IsChecked = isChecked;
        }

        private static (bool isVisible, bool? isChecked) GetCbxSubfolderProperties(SubfolderSelectionType sel, SubfolderType sub)
        {
            bool isVisible;
            bool? isChecked;

            if ((sel == SubfolderSelectionType.Auto && sub == SubfolderType.No) ||
                (sel == SubfolderSelectionType.Hidden) ||
                (sel == SubfolderSelectionType.NoAll && sub == SubfolderType.This) ||
                (sel == SubfolderSelectionType.NoThis && sub == SubfolderType.All) ||
                (sel == SubfolderSelectionType.ThisAll && sub == SubfolderType.No))
            {
                isVisible = false;
            }
            else isVisible = true;


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

            return (isVisible, isChecked);
        }
    }
}
