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

        private Models.SettingsModel _settings;

        private IList _selectedAlerts;

        private Models.AlertModel _visibleAlert;

        #endregion Fields

        public AlertsViewModel(List<Models.AlertModel> alerts, Models.SettingsModel Settings, EventAggregator eventAggregator)
        {
            _settings = Settings;

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

        #region Commands

        public DelegateCommand<Models.AlertModel> RemoveCommand { get; set; }

        public DelegateCommand RemoveSelectedCommand { get; set; }

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand<IList> SelectionChangedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        #endregion Commands

        #region Other properties

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

        public Models.SettingsModel Settings
        {
            get
            {
                return _settings;
            }
        }

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

        public Models.AlertModel VisibleAlert
        {
            get
            {
                return _visibleAlert;
            }
            set
            {
                SetProperty(ref _visibleAlert, value);
            }
        }

        #endregion Other properties

        #region Methods

        private void AddAlert(Models.AlertModel alert)
        {
            Models.AlertModel alertCopy = alert.Clone() as Models.AlertModel;

            if (!alertCopy.Time.Offset.Equals(_settings.Alerts.TimeZone.BaseUtcOffset))
            {
                alertCopy.Time = alert.Time.ToOffset(_settings.Alerts.TimeZone.BaseUtcOffset);
            }

            alertCopy.Price = Math.Round(alertCopy.Price, Settings.Alerts.MaxPriceDecimalPlacesNumber);

            Alerts.Add(alertCopy);

            VisibleAlert = alertCopy;
        }

        private void AlertAddedEvent_Handler(Models.AlertModel alert)
        {
            AddAlert(alert);

            Cleanup();
        }

        private void Cleanup()
        {
            if (Alerts.Count > _settings.Alerts.MaxAlertNumber)
            {
                Alerts.OrderByDescending(alert => alert.Time).Skip(_settings.Alerts.MaxAlertNumber).ToList().ForEach(alert => Remove(alert));
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