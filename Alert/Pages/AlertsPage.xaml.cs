using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Alert.Pages
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

            Resources.MergedDictionaries.Add(Factory.CurrentAccent.Resources);
            Resources.MergedDictionaries.Add(Factory.CurrentTheme.Resources);

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
                Factory.WriteAlerts(new List<Alert>(), FileMode.Create);

                alertsModel.Alerts.Clear();
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
                if (alertsModel.SelectedAlert != null)
                {
                    alertsModel.Alerts.Remove(alertsModel.SelectedAlert);

                    Factory.WriteAlerts(alertsModel.Alerts, FileMode.Create);
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
                alertsModel.Alerts = new ObservableCollection<Alert>(Factory.ReadAlerts());

                if (alertsModel.Alerts.Count > Factory.MaximumAlertsNumberToShow)
                {
                    File.WriteAllText(Factory.FilePath, string.Empty);

                    int counter = alertsModel.Alerts.Count - Factory.MaximumAlertsNumberToShow;

                    alertsModel.Alerts.ToList().ForEach(alert =>
                    {
                        if (counter > 0)
                        {
                            alertsModel.Alerts.Remove(alert);
                            counter--;
                        }
                    });

                    Factory.WriteAlerts(alertsModel.Alerts.ToList());
                }

                alertsModel.SelectedAlert = alertsModel.Alerts.LastOrDefault();

                AlertsDataGrid.UpdateLayout();
                AlertsDataGrid.ScrollIntoView(alertsModel.SelectedAlert, null);
            }
            catch (Exception ex)
            {
                Factory.LogException(ex);
            }
        }

        #endregion Methods
    }
}