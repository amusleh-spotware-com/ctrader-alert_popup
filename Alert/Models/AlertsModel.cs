using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Alert.Models
{
    public class AlertsModel : INotifyPropertyChanged
    {
        #region Fields

        private Alert _selectedAlert;

        private ObservableCollection<Alert> _alerts;

        #endregion Fields

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

        public static string CurrentTimeFormat
        {
            get
            {
                return Registry.GetValue("CurrentTimeFormat", "12 Hour");
            }
        }

        #endregion Properties

        #region Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods
    }
}