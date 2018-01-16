using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Windows;

namespace Alert
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        #region Constructor

        public MainWindow()
        {
            Application.ResourceAssembly = typeof(MainWindow).Assembly;

            Loaded += MetroWindow_Loaded;
            Closing += MetroWindow_Closing;

            InitializeComponent();

            Resources.MergedDictionaries.Add(Factory.CurrentAccent.Resources);
            Resources.MergedDictionaries.Add(Factory.CurrentTheme.Resources);

            Content = new Pages.AlertsPage();
        }

        #endregion Constructor

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Methods

        public void Invoke(Action action)
        {
            this.Dispatcher.BeginInvoke(action);
        }

        private void BringInFront()
        {
            if (this.Visibility == Visibility.Hidden || this.Visibility == Visibility.Collapsed)
            {
                this.Visibility = Visibility.Visible;
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Factory.CloseOpenWindows();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsButton.Visibility = Visibility.Collapsed;

            HomeButton.Visibility = Visibility.Visible;

            this.Content = new Pages.SettingsPage();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            HomeButton.Visibility = Visibility.Collapsed;

            SettingsButton.Visibility = Visibility.Visible;

            this.Content = new Pages.AlertsPage();
        }

        #endregion Methods
    }
}