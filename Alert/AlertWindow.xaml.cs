using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace Alert
{
    public partial class AlertWindow : MetroWindow, INotifyPropertyChanged
    {
        #region Fields

        private Alert _selectedAlert;

        private ObservableCollection<Alert> _alerts = new ObservableCollection<Alert>();

        #endregion Fields

        #region Constructor

        public AlertWindow()
        {
            Application.ResourceAssembly = typeof(AlertWindow).Assembly;

            Loaded += MetroWindow_Loaded;
            Closing += MetroWindow_Closing;

            InitializeComponent();

            ThemeManager.ChangeAppStyle(this, Factory.CurrentAccent, Factory.CurrentTheme);
        }

        #endregion Constructor

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public ObservableCollection<Alert> Alerts
        {
            get
            {
                return _alerts;
            }
            set
            {
                _alerts = value;

                OnPropertyChanged("Alerts");
            }
        }

        public Alert SelectedAlert
        {
            get
            {
                return _selectedAlert;
            }
            set
            {
                _selectedAlert = value;

                OnPropertyChanged("SelectedAlert");
            }
        }

        #endregion Properties

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
            GetAlertsFromFile();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Factory.CloseOpenAlertWindows();
        }

        private void RemoveAllAlertsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Factory.WriteAlerts(new List<Alert>(), FileMode.Create);

                Alerts.Clear();
            }
            catch (Exception ex)
            {
                Factory.LogException(ex);
            }
        }

        private void RemoveAlertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedAlert != null)
                {
                    Alerts.Remove(SelectedAlert);

                    Factory.WriteAlerts(Alerts, FileMode.Create);
                }
            }
            catch (Exception ex)
            {
                Factory.LogException(ex);
            }
        }

        private void GetAlertsFromFile()
        {
            try
            {
                Alerts = new ObservableCollection<Alert>(Factory.ReadAlerts());

                if (Alerts.Count > Factory.MaximumAlertsNumberToShow)
                {
                    File.WriteAllText(Factory.FilePath, string.Empty);

                    int counter = Alerts.Count - Factory.MaximumAlertsNumberToShow;

                    Alerts.ToList().ForEach(alert =>
                    {
                        if (counter > 0)
                        {
                            Alerts.Remove(alert);
                            counter--;
                        }
                    });

                    Factory.WriteAlerts(Alerts.ToList());
                }

                SelectedAlert = Alerts.LastOrDefault();

                AlertsDataGrid.UpdateLayout();
                AlertsDataGrid.ScrollIntoView(SelectedAlert, null);
            }
            catch (Exception ex)
            {
                Factory.LogException(ex);

                File.Delete(Factory.FilePath);
                File.Create(Factory.FilePath).Close();
            }

            BringInFront();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();

            this.IsEnabled = false;

            settingsWindow.ShowDialog();

            ThemeManager.ChangeAppStyle(this, Factory.CurrentAccent, Factory.CurrentTheme);

            GetAlertsFromFile();

            this.IsEnabled = true;
        }

        #endregion Methods
    }
}