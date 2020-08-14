using cAlgo.API.Alert.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cAlgo.API.Alert.ViewModels
{
    public class AlertsViewModel : BindableBase
    {
        #region Fields

        private ObservableCollection<AlertModel> _alerts;

        private readonly EventAggregator _eventAggregator;

        private readonly SettingsModel _settings;

        private IList _selectedAlerts;

        private AlertModel _visibleAlert;

        #endregion Fields

        public AlertsViewModel(IEnumerable<AlertModel> alerts, SettingsModel Settings, EventAggregator eventAggregator)
        {
            _settings = Settings;

            _eventAggregator = eventAggregator;

            Alerts = new ObservableCollection<AlertModel>();

            var alertsList = alerts.ToList();

            alertsList.ForEach(alert => AddAlert(alert));

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            SelectionChangedCommand = new DelegateCommand<IList>(SelectionChanged);

            RemoveCommand = new DelegateCommand<AlertModel>(Remove);

            RemoveSelectedCommand = new DelegateCommand(RemoveSelected, () => SelectedAlerts != null && SelectedAlerts.Count > 0)
                .ObservesProperty(() => SelectedAlerts);
        }

        #region Commands

        public DelegateCommand<AlertModel> RemoveCommand { get; set; }

        public DelegateCommand RemoveSelectedCommand { get; set; }

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand<IList> SelectionChangedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        #endregion Commands

        #region Other properties

        public ObservableCollection<AlertModel> Alerts
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

        public SettingsModel Settings
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

        public AlertModel VisibleAlert
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

        private void AddAlert(AlertModel alert)
        {
            AlertModel alertCopy = alert.Clone() as AlertModel;

            if (!alertCopy.Time.Offset.Equals(_settings.Alerts.TimeZone.BaseUtcOffset))
            {
                alertCopy.Time = alert.Time.ToOffset(_settings.Alerts.TimeZone.BaseUtcOffset);
            }

            alertCopy.Price = Math.Round(alertCopy.Price, Settings.Alerts.MaxPriceDecimalPlacesNumber);

            if (!Alerts.Contains(alertCopy))
            {
                Alerts.Add(alertCopy);

                VisibleAlert = alertCopy;
            }
        }

        private void AlertAddedEvent_Handler(AlertModel alert)
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

        private void Remove(AlertModel alert)
        {
            RemoveAlerts(new AlertModel[] { alert });
        }

        private void RemoveSelected()
        {
            var alertsToRemove = SelectedAlerts.Cast<AlertModel>().ToArray();

            RemoveAlerts(alertsToRemove);
        }

        private void RemoveAlerts(IEnumerable<AlertModel> alerts)
        {
            foreach (var alert in alerts)
            {
                if (Alerts.Contains(alert))
                {
                    Alerts.Remove(alert);
                }
            }

            _eventAggregator.GetEvent<Events.AlertRemovedEvent>().Publish(alerts);
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