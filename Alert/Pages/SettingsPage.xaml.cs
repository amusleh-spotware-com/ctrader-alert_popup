using MahApps.Metro;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Alert.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        #region Fields

        private Models.SettingsModel settingsModel = new Models.SettingsModel();

        #endregion

        #region Constructors

        public SettingsPage()
        {
            Loaded += SettingsPage_Loaded;
            Unloaded += SettingsPage_Unloaded;

            InitializeComponent();

            Resources.MergedDictionaries.Add(Factory.CurrentAccent.Resources);
            Resources.MergedDictionaries.Add(Factory.CurrentTheme.Resources);

            DataContext = settingsModel;
        }

        #endregion Constructors

        #region Methods

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void SettingsPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public void Invoke(Action action)
        {
            this.Dispatcher.BeginInvoke(action);
        }

        private void SoundFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                settingsModel.SoundFilePath = openFileDialog.FileName;
            }
        }

        private void ThemeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (settingsModel.CurrentAccent != null && settingsModel.CurrentTheme != null)
            {
                Resources.MergedDictionaries.Add(settingsModel.CurrentAccent.Resources);
                Resources.MergedDictionaries.Add(settingsModel.CurrentTheme.Resources);

                // Main window
                Factory.Window.Resources.MergedDictionaries.Add(settingsModel.CurrentAccent.Resources);
                Factory.Window.Resources.MergedDictionaries.Add(settingsModel.CurrentTheme.Resources);
            }
        }

        #endregion Methods
    }
}