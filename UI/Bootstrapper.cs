using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MahApps.Metro;

namespace cAlgo.API.Alert.UI
{
    public class Bootstrapper
    {
        #region Fields

        private string _currentView;

        private readonly Views.ShellView _shellView;

        private readonly List<string> _navigationJournal;

        private readonly ObservableCollection<Models.AlertModel> _alerts;

        private readonly Models.OptionsModel _options;

        #endregion Fields

        #region Delegates

        public delegate void AlertChangeHandler(Models.AlertModel alert);

        public delegate void OptionsChangedHandler(Models.OptionsModel options);

        #endregion Delegates

        #region Events

        public event AlertChangeHandler AlertAddedEvent;

        public event AlertChangeHandler AlertRemovedEvent;

        public event OptionsChangedHandler OptionsChangedEvent;

        #endregion Events

        #region Constructors

        public Bootstrapper()
        {
            _shellView = CreateView<Views.ShellView, ViewModels.ShellViewModel>(this);

            _navigationJournal = new List<string>();

            _alerts = new ObservableCollection<Models.AlertModel>();

            _options = new Models.OptionsModel
            {
                TimeFormat = Enums.TimeFormat.TwentyFourHour,
                TimeFrameColor = Brushes.DarkMagenta,
                TimeColor = Brushes.DimGray,
                TimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(tz => tz.BaseUtcOffset.Equals(new TimeSpan(0, 0, 0))),
                ThemeBase = ThemeManager.AppThemes.FirstOrDefault(
                    theme => theme.Name.Equals("Light", StringComparison.InvariantCultureIgnoreCase)),
                ThemeAccent = ThemeManager.Accents.FirstOrDefault(
                    accent => accent.Name.Equals("Blue", StringComparison.InvariantCultureIgnoreCase)),
                BuySideColor = Brushes.Green,
                SellSideColor = Brushes.Red,
                SymbolColor = Brushes.DarkGoldenrod,
                TriggeredByColor = Brushes.DeepPink,
            };

            OptionsChangedEvent += Bootstrapper_OptionsChangedEvent;
        }

        #endregion Constructors

        #region Properties

        public Window ShellView
        {
            get
            {
                return _shellView;
            }
        }

        public string CurrentView
        {
            get
            {
                return _currentView;
            }
        }

        public List<string> NavigationJournal
        {
            get
            {
                return _navigationJournal;
            }
        }

        public Models.OptionsModel Options
        {
            get
            {
                return _options;
            }
        }

        public ResourceDictionary SharedResources
        {
            get
            {
                return _shellView.Resources.MergedDictionaries.FirstOrDefault(resource =>
                resource.Source.AbsolutePath.IndexOf("SharedResources.xaml", StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
        }

        #endregion Properties

        #region Methods

        public void Run()
        {
            _shellView.ShowDialog();
        }

        public void Shutdown()
        {
            _shellView.Close();
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
                    _shellView.Content = CreateView<Views.AlertsView, ViewModels.AlertsViewModel>(_alerts, _options);
                    break;

                case ViewNames.OptionsView:
                    _shellView.Content = CreateView<Views.OptionsView, ViewModels.OptionsViewModel>(_options);
                    break;
            }
        }

        public void AddAlert(Models.AlertModel alert)
        {
            _alerts.Add(alert);

            AlertAddedEvent?.Invoke(alert);
        }

        public void RemoveAlert(Models.AlertModel alert)
        {
            _alerts.Remove(alert);

            AlertRemovedEvent?.Invoke(alert);
        }

        public List<Models.AlertModel> GetAlerts()
        {
            return _alerts.ToList();
        }

        private ResourceDictionary GetMetroResource(string name)
        {
            Uri uri = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", name));

            return new ResourceDictionary() { Source = uri };
        }

        private T GetViewModel<T>(params object[] parameters) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        private TView CreateView<TView, TViewModel>(params object[] parameters) where TView : class
            where TViewModel : class
        {
            TView view = (TView)Activator.CreateInstance(typeof(TView));

            (view as FrameworkElement).DataContext = GetViewModel<TViewModel>(parameters);

            return view;
        }

        private void Bootstrapper_OptionsChangedEvent(Models.OptionsModel options)
        {
            ThemeManager.ChangeAppStyle(SharedResources, options.ThemeAccent, options.ThemeBase);
        }

        #endregion Methods
    }
}