using cAlgo.API.Alert.UI.Factories;
using cAlgo.API.Alert.UI.Models;
using MahApps.Metro;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cAlgo.API.Alert.UI
{
    public class App
    {
        #region Fields

        private readonly List<Models.AlertModel> _alerts;

        private readonly EventAggregator _eventAggregator;

        private readonly List<string> _navigationJournal;

        private readonly Views.ShellView _shellView;

        private readonly ResourceDictionary _themeResources;

        private readonly string _settingsFilePath;

        private readonly Models.SettingsModel _settings;

        private string _currentView;

        #endregion Fields

        public App(string settingsFilePath, List<AlertModel> alerts)
        {
            _settingsFilePath = settingsFilePath;

            _settings = SettingsFactory.GetSettings(_settingsFilePath);

            _themeResources = new ResourceDictionary();

            _shellView = CreateView<Views.ShellView, ViewModels.ShellViewModel>(this);

            _shellView.Topmost = _settings.General.TopMost;

            Accent accent = SettingsFactory.GetAccent(_settings.General.ThemeAccent);
            AppTheme theme = SettingsFactory.GetTheme(_settings.General.ThemeBase);

            _themeResources.MergedDictionaries.Add(accent.Resources);
            _themeResources.MergedDictionaries.Add(theme.Resources);

            _navigationJournal = new List<string>();

            _eventAggregator = new EventAggregator();

            _eventAggregator.GetEvent<Events.SettingsChangedEvent>().Subscribe(SettingsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.GeneralSettingsChangedEvent>().Subscribe(GeneralSettingsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.AlertRemovedEvent>().Subscribe(AlertRemovedEvent_Handler);

            _alerts = alerts;
        }

        #region Properties

        public string CurrentView
        {
            get
            {
                return _currentView;
            }
        }

        public EventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }
        }

        public List<string> NavigationJournal
        {
            get
            {
                return _navigationJournal;
            }
        }

        public Models.SettingsModel Settings
        {
            get
            {
                return _settings;
            }
        }

        public string SettingsFilePath
        {
            get
            {
                return _settingsFilePath;
            }
        }

        public Window ShellView
        {
            get
            {
                return _shellView;
            }
        }

        public ResourceDictionary ThemeResources
        {
            get
            {
                return _themeResources;
            }
        }

        public bool IsWindowOpen
        {
            get
            {
                return ShellView != null && ShellView.Dispatcher.Invoke(() => ShellView.Visibility != Visibility.Hidden);
            }
        }

        #endregion Properties

        #region Methods

        public void AlertRemovedEvent_Handler(IEnumerable<AlertModel> alerts)
        {
            foreach (var alert in alerts)
            {
                if (_alerts.Contains(alert))
                {
                    _alerts.Remove(alert);
                }
            }
        }

        public List<Models.AlertModel> GetAlerts()
        {
            return _alerts.ToList();
        }

        public void InvokeAlertAddedEvent(Models.AlertModel alert)
        {
            if (ShellView != null)
            {
                InvokeOnWindowThread(() =>
                {
                    EventAggregator.GetEvent<Events.AlertAddedEvent>().Publish(alert);

                    if (ShellView.WindowState == WindowState.Minimized)
                    {
                        ShellView.WindowState = WindowState.Normal;
                    }

                    if (!ShellView.Topmost)
                    {
                        ShellView.Topmost = true;
                        ShellView.Topmost = false;
                    }
                });
            }
            else
            {
                EventAggregator.GetEvent<Events.AlertAddedEvent>().Publish(alert);
            }
        }

        public void InvokeOnWindowThread(Action action)
        {
            ShellView.Dispatcher.Invoke(action);
        }

        public void Navigate(string viewName)
        {
            if (viewName.Equals(_currentView, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            _currentView = viewName;

            _navigationJournal.Add(_currentView);

            switch (viewName)
            {
                case ViewNames.AlertsView:
                    _shellView.Content = CreateView<Views.AlertsView, ViewModels.AlertsViewModel>(_alerts, _settings, _eventAggregator);
                    break;

                case ViewNames.SettingsView:
                    _shellView.Content = CreateView<Views.SettingsView, ViewModels.SettingsViewModel>(_settings, _eventAggregator);
                    break;

                case ViewNames.AboutView:
                    _shellView.Content = CreateView<Views.AboutView, ViewModels.AboutViewModel>();
                    break;
            }
        }

        public void Run()
        {
            InvokeOnWindowThread(() =>
            {
                if (_shellView.Visibility != Visibility.Visible)
                {
                    _shellView.ShowDialog();
                }
            });
        }

        public void Shutdown()
        {
            _shellView.Close();
        }

        private TView CreateView<TView, TViewModel>(params object[] parameters) where TView : class
            where TViewModel : class
        {
            TView view = (TView)Activator.CreateInstance(typeof(TView));

            (view as ContentControl).Resources.MergedDictionaries.Add(_themeResources);

            (view as FrameworkElement).DataContext = GetViewModel<TViewModel>(parameters);

            return view;
        }

        private T GetViewModel<T>(params object[] parameters) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        private void GeneralSettingsChangedEvent_Handler(Models.GeneralSettingsModel settings)
        {
            _shellView.Topmost = settings.TopMost;

            Accent accent = SettingsFactory.GetAccent(settings.ThemeAccent);
            AppTheme theme = SettingsFactory.GetTheme(settings.ThemeBase);

            ThemeManager.ChangeAppStyle(_themeResources, accent, theme);
        }

        private void SettingsChangedEvent_Handler(Models.SettingsModel settings)
        {
            if (!string.IsNullOrEmpty(_settingsFilePath))
            {
                SettingsFactory.SaveSettings(_settingsFilePath, Settings);
            }
        }

        #endregion Methods
    }
}