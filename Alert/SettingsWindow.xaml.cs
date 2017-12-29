namespace Alert
{
    using MahApps.Metro.Controls;
    using Nortal.Utilities.Csv;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using Microsoft.Win32;
    using System.ComponentModel;
    using MahApps.Metro;
    using System.Linq;

    public partial class SettingsWindow : MetroWindow, INotifyPropertyChanged
    {
        #region Fields

        #endregion

        #region Constructor
        public SettingsWindow()
        {
            Loaded += MetroWindow_Loaded;
            Closing += MetroWindow_Closing;

            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Alert;component/Resources/Icons.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", Manager.CurrentAccent.Name), UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", Manager.CurrentTheme.Name), UriKind.RelativeOrAbsolute) });

            InitializeComponent();
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public bool IsSoundAlertEnabled
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsSoundAlertEnabled", false));
            }
            set
            {
                Registry.SetValue("IsSoundAlertEnabled", value);

                OnPropertyChanged(new PropertyChangedEventArgs("IsSoundAlertEnabled"));
            }
        }

        public string SoundFilePath
        {
            get
            {
                return Registry.GetValue("SoundFilePath", string.Empty);
            }
            set
            {
                Registry.SetValue("SoundFilePath", value);

                OnPropertyChanged(new PropertyChangedEventArgs("SoundFilePath"));
            }
        }

        public bool IsEmailAlertEnabled
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsEmailAlertEnabled", false));
            }
            set
            {
                Registry.SetValue("IsEmailAlertEnabled", value);

                OnPropertyChanged(new PropertyChangedEventArgs("IsEmailAlertEnabled"));
            }
        }

        public string FromEmail
        {
            get
            {
                return Registry.GetValue("FromEmail", "");
            }
            set
            {
                Registry.SetValue("FromEmail", value);

                OnPropertyChanged(new PropertyChangedEventArgs("FromEmail"));
            }
        }

        public string ToEmail
        {
            get
            {
                return Registry.GetValue("ToEmail", string.Empty);
            }
            set
            {
                Registry.SetValue("ToEmail", value);

                OnPropertyChanged(new PropertyChangedEventArgs("ToEmail"));
            }
        }

        public AppTheme CurrentTheme
        {
            get
            {
                return Manager.CurrentTheme;
            }
            set
            {
                Registry.SetValue("CurrentTheme", value.Name);

                OnPropertyChanged(new PropertyChangedEventArgs("CurrentTheme"));
            }
        }

        public Accent CurrentAccent
        {
            get
            {
                return Manager.CurrentAccent;
            }
            set
            {
                Registry.SetValue("CurrentAccent", value.Name);

                OnPropertyChanged(new PropertyChangedEventArgs("CurrentAccent"));
            }
        }

        public ObservableCollection<AppTheme> Themes
        {
            get
            {
                return new ObservableCollection<AppTheme>(ThemeManager.AppThemes);
            }
        }

        public ObservableCollection<Accent> Accents
        {
            get
            {
                return new ObservableCollection<Accent>(ThemeManager.Accents);
            }
        }
        #endregion

        #region Methods
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        public void Invoke(Action action)
        {
            this.Dispatcher.BeginInvoke(action);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void RefreshCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void SoundFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == true)
            {
                SoundFilePath = openFileDialog.FileName;
            }
        }

        private void ThemeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AccentsComboBox.SelectedItem != null && ThemesComboBox.SelectedItem != null)
            {
                ThemeManager.ChangeAppStyle(this, (Accent)AccentsComboBox.SelectedItem, (AppTheme)ThemesComboBox.SelectedItem);
            }
        }
        #endregion
    }
}
