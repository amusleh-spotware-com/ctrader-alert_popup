using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class AlertsViewModel : BindableBase
    {
        #region Fields

        private ObservableCollection<Models.AlertModel> _alerts;

        private EventAggregator _eventAggregator;
        private Models.OptionsModel _options;
        private IList _selectedAlerts;

        #endregion Fields

        #region Constructor

        public AlertsViewModel(List<Models.AlertModel> alerts, Models.OptionsModel options, EventAggregator eventAggregator)
        {
            _options = options;

            _eventAggregator = eventAggregator;

            Alerts = new ObservableCollection<Models.AlertModel>();

            alerts.ForEach(alert => AddAlert(alert));

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            SelectionChangedCommand = new DelegateCommand<IList>(SelectionChanged);

            RemoveCommand = new DelegateCommand<Models.AlertModel>(Remove);

            RemoveSelectedCommand = new DelegateCommand(RemoveSelected, () => SelectedAlerts != null && SelectedAlerts.Count > 0)
                .ObservesProperty(() => SelectedAlerts);
        }

        #endregion Constructor

        #region Properties

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

        public DelegateCommand LoadedCommand { get; set; }

        public Models.OptionsModel Options
        {
            get
            {
                return _options;
            }
        }

        public DelegateCommand<Models.AlertModel> RemoveCommand { get; set; }
        public DelegateCommand RemoveSelectedCommand { get; set; }

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

        public DelegateCommand<IList> SelectionChangedCommand { get; set; }
        public DelegateCommand UnloadedCommand { get; set; }

        #endregion Properties

        #region Methods

        private void AddAlert(Models.AlertModel alert)
        {
            Models.AlertModel alertCopy = alert.Clone() as Models.AlertModel;

            if (!alertCopy.Time.Offset.Equals(_options.Alerts.TimeZone.BaseUtcOffset))
            {
                alertCopy.Time = alert.Time.ToOffset(_options.Alerts.TimeZone.BaseUtcOffset);
            }

            alertCopy.Price = Math.Round(alertCopy.Price, Options.Alerts.MaxPriceDecimalPlacesNumber);

            Alerts.Add(alertCopy);
        }

        private void AlertAddedEvent_Handler(Models.AlertModel alert)
        {
            AddAlert(alert);

            Cleanup();
        }

        private void Cleanup()
        {
            if (Alerts.Count > _options.Alerts.MaxAlertNumber)
            {
                Alerts.OrderByDescending(alert => alert.Time).Skip(_options.Alerts.MaxAlertNumber).ToList().ForEach(alert => Remove(alert));
            }
        }

        private void Loaded()
        {
            Cleanup();

            _eventAggregator.GetEvent<Events.AlertAddedEvent>().Subscribe(AlertAddedEvent_Handler);
        }

        private void Remove(Models.AlertModel alert)
        {
            if (Alerts.Contains(alert))
            {
                _eventAggregator.GetEvent<Events.AlertRemovedEvent>().Publish(alert);

                Alerts.Remove(alert);
            }
        }

        private void RemoveSelected()
        {
            SelectedAlerts.Cast<Models.AlertModel>().ToList().ForEach(alert => Remove(alert));
        }

        private void SelectionChanged(IList selectedItems)
        {
            SelectedAlerts = selectedItems;
        }

        private void Unloaded()
        {
        }

        #endregion Methods
    }
}