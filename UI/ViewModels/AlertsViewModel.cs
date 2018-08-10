using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using Prism.Events;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class AlertsViewModel : BindableBase
    {
        #region Fields

        private ObservableCollection<Models.AlertModel> _alerts;

        private IList _selectedAlerts;

        private Models.OptionsModel _options;

        private EventAggregator _eventAggregator;

        #endregion Fields

        #region Constructor

        public AlertsViewModel(ObservableCollection<Models.AlertModel> alerts, Models.OptionsModel options, EventAggregator eventAggregator)
        {
            Alerts = alerts;

            _options = options;

            _eventAggregator = eventAggregator;

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            SelectionChangedCommand = new DelegateCommand<IList>(SelectionChanged);

            RemoveCommand = new DelegateCommand<Models.AlertModel>(Remove);

            RemoveSelectedCommand = new DelegateCommand(RemoveSelected, () => SelectedAlerts != null && SelectedAlerts.Count > 0)
                .ObservesProperty(() => SelectedAlerts);
        }

        #endregion Constructor

        #region Properties

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        public DelegateCommand RemoveSelectedCommand { get; set; }

        public DelegateCommand<Models.AlertModel> RemoveCommand { get; set; }

        public ObservableCollection<Models.AlertModel> Alerts
        {
            get
            {
                return _alerts;
            }
            set
            {
                SetProperty(ref _alerts, value);
            }
        }

        public DelegateCommand<IList> SelectionChangedCommand { get; set; }

        public IList SelectedAlerts
        {
            get
            {
                return _selectedAlerts;
            }
            set
            {
                SetProperty(ref _selectedAlerts, value);
            }
        }

        public Models.OptionsModel Options
        {
            get
            {
                return _options;
            }
        }

        #endregion Properties

        #region Methods

        private void Loaded()
        {
            if (Alerts.Count > _options.Alerts.MaxAlertNumber)
            {
                Alerts.OrderByDescending(alert => alert.Time).Skip(_options.Alerts.MaxAlertNumber).ToList().ForEach(alert => Remove(alert));
            }

            // Changing the alerts time zone
            Alerts.Where(alert => !alert.Time.Offset.Equals(_options.Alerts.TimeZone.BaseUtcOffset)).ToList().ForEach(alert =>
            {
                alert.Time = alert.Time.ToOffset(_options.Alerts.TimeZone.BaseUtcOffset);
            });
        }

        private void Unloaded()
        {
        }

        private void SelectionChanged(IList selectedItems)
        {
            SelectedAlerts = selectedItems;
        }

        private void RemoveSelected()
        {
            SelectedAlerts.Cast<Models.AlertModel>().ToList().ForEach(alert => Remove(alert));
        }

        private void Remove(Models.AlertModel alert)
        {
            if (Alerts.Contains(alert))
            {
                Alerts.Remove(alert);
            }
        }

        #endregion Methods
    }
}