using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cAlgo.API.Alert.Pages
{
    /// <summary>
    /// Interaction logic for AlertsPage.xaml
    /// </summary>
    public partial class AlertsPage : Page
    {
        #region Fields

        private Models.AlertsModel alertsModel = new Models.AlertsModel();

        #endregion Fields

        #region Constructors

        public AlertsPage()
        {
            Loaded += AlertsPage_Loaded;
            Unloaded += AlertsPage_Unloaded;

            InitializeComponent();

            Resources.MergedDictionaries.Add(INotificationsExtensions.GetStyleResource(INotificationsExtensions.CurrentTheme));
            Resources.MergedDictionaries.Add(INotificationsExtensions.GetStyleResource(INotificationsExtensions.CurrentAccent));

            DataContext = alertsModel;
        }

        #endregion Constructors

        #region Methods

        private void AlertsPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetAlertsFromFile();
        }

        private void AlertsPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public void Invoke(Action action)
        {
            this.Dispatcher.BeginInvoke(action);
        }

        private void RemoveAllAlertsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                INotificationsExtensions.WriteAlerts(new List<Alert>(), FileMode.Create);

                alertsModel.Alerts.Clear();
            }
            catch (Exception ex)
            {
                INotificationsExtensions.LogException(ex);
            }
        }

        private void RemoveAlertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (alertsModel.SelectedAlert != null)
                {
                    alertsModel.Alerts.Remove(alertsModel.SelectedAlert);

                    INotificationsExtensions.WriteAlerts(alertsModel.Alerts, FileMode.Create);
                }
            }
            catch (Exception ex)
            {
                INotificationsExtensions.LogException(ex);
            }
        }

        private void GetAlertsFromFile()
        {
            try
            {
                List<Alert> alerts = INotificationsExtensions.ReadAlerts();

                if (alerts.Count > INotificationsExtensions.MaximumAlertsNumberToShow)
                {
                    File.WriteAllText(INotificationsExtensions.FilePath, string.Empty);

                    int counter = alerts.Count - INotificationsExtensions.MaximumAlertsNumberToShow;

                    alerts.ToList().ForEach(alert =>
                    {
                        if (counter > 0)
                        {
                            alerts.Remove(alert);
                            counter--;
                        }
                    });

                    INotificationsExtensions.WriteAlerts(alerts.ToList());
                }

                alerts.ForEach(alert => alert.Time = alert.Time.ToOffset(INotificationsExtensions.CurrentTimeZone.BaseUtcOffset));

                alertsModel.Alerts = new ObservableCollection<Alert>(alerts);
                alertsModel.SelectedAlert = alertsModel.Alerts.LastOrDefault();

                AlertsDataGrid.UpdateLayout();
                AlertsDataGrid.ScrollIntoView(alertsModel.SelectedAlert, null);
            }
            catch (Exception ex)
            {
                INotificationsExtensions.LogException(ex);
            }
        }

        #endregion Methods
    }
}