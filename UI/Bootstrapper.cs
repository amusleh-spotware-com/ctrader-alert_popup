using MahApps.Metro;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace cAlgo.API.Alert.UI
{
    public class Bootstrapper
    {
        #region Fields

        private string _currentView;

        private readonly Views.ShellView _shellView;

        private readonly List<string> _navigationJournal;

        private readonly ObservableCollection<Models.AlertModel> _alerts;

        private Models.OptionsModel _options;

        private readonly EventAggregator _eventAggregator;

        private readonly ResourceDictionary _themeResources;

        #endregion Fields

        #region Constructors

        public Bootstrapper()
        {
            _themeResources = new ResourceDictionary();

            _navigationJournal = new List<string>();

            _alerts = new ObservableCollection<Models.AlertModel>();

            _eventAggregator = new EventAggregator();

            _eventAggregator.GetEvent<Events.OptionsChangedEvent>().Subscribe(OptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.GeneralOptionsChangedEvent>().Subscribe(GeneralOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.AlertOptionsChangedEvent>().Subscribe(AlertOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.SoundOptionsChangedEvent>().Subscribe(SoundOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.EmailOptionsChangedEvent>().Subscribe(EmailOptionsChangedEvent_Handler);

            _shellView = CreateView<Views.ShellView, ViewModels.ShellViewModel>(this);
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

        public ResourceDictionary ThemeResources
        {
            get
            {
                return _themeResources;
            }
        }

        public EventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }
        }

        #endregion Properties

        #region Methods

        public void Run(Models.OptionsModel options)
        {
            _options = options;

            _themeResources.MergedDictionaries.Add(_options.General.ThemeAccent.Accent.Resources);
            _themeResources.MergedDictionaries.Add(_options.General.ThemeBase.Base.Resources);

            _shellView.ShowDialog();
        }

        public void Run()
        {
            Models.OptionsModel options = ViewModels.OptionsBaseViewModel.GetDefaultOptions();

            Run(options);
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
                    _shellView.Content = CreateView<Views.AlertsView, ViewModels.AlertsViewModel>(_alerts, _options, _eventAggregator);
                    break;

                case ViewNames.OptionsView:
                    _shellView.Content = CreateView<Views.OptionsView, ViewModels.OptionsViewModel>(_options, _eventAggregator);
                    break;
            }
        }

        public void AddAlert(Models.AlertModel alert)
        {
            _alerts.Add(alert);

            EventAggregator.GetEvent<Events.AlertAddedEvent>().Publish(alert);
        }

        public void RemoveAlert(Models.AlertModel alert)
        {
            _alerts.Remove(alert);

            EventAggregator.GetEvent<Events.AlertRemovedEvent>().Publish(alert);
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

            (view as ContentControl).Resources.MergedDictionaries.Add(_themeResources);

            (view as FrameworkElement).DataContext = GetViewModel<TViewModel>(parameters);

            return view;
        }

        private void OptionsChangedEvent_Handler(Models.OptionsModel options)
        {
        }

        private void GeneralOptionsChangedEvent_Handler(Models.GeneralOptionsModel options)
        {
            ThemeManager.ChangeAppStyle(_themeResources, options.ThemeAccent.Accent, options.ThemeBase.Base);
        }

        private void EmailOptionsChangedEvent_Handler(Models.EmailOptionsModel options)
        {
        }

        private void SoundOptionsChangedEvent_Handler(Models.SoundOptionsModel options)
        {
        }

        private void AlertOptionsChangedEvent_Handler(Models.AlertOptionsModel options)
        {
        }

        #endregion Methods
    }
}