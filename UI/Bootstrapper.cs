using CsvHelper;
using MahApps.Metro;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI
{
    public class Bootstrapper
    {
        #region Fields

        private string _currentView;

        private readonly Views.ShellView _shellView;

        private readonly List<string> _navigationJournal;

        private readonly List<Models.AlertModel> _alerts;

        private Models.OptionsModel _options;

        private readonly EventAggregator _eventAggregator;

        private readonly ResourceDictionary _themeResources;

        private readonly string _alertsFilePath;

        private string _optionsFilePath;

        #endregion Fields

        #region Constructors

        public Bootstrapper(string alertsFilePath)
        {
            _alertsFilePath = alertsFilePath;

            _themeResources = new ResourceDictionary();

            _navigationJournal = new List<string>();

            _alerts = new List<Models.AlertModel>();

            _eventAggregator = new EventAggregator();

            _eventAggregator.GetEvent<Events.OptionsChangedEvent>().Subscribe(OptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.GeneralOptionsChangedEvent>().Subscribe(GeneralOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.AlertOptionsChangedEvent>().Subscribe(AlertOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.SoundOptionsChangedEvent>().Subscribe(SoundOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.EmailOptionsChangedEvent>().Subscribe(EmailOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.TelegramOptionsChangedEvent>().Subscribe(TelegramOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.AlertRemovedEvent>().Subscribe(AlertRemovedEvent_Handler);

            _shellView = CreateView<Views.ShellView, ViewModels.ShellViewModel>(this);

            if (File.Exists(_alertsFilePath))
            {
                List<Models.AlertModel> fileAlerts = GetAlertsFromFile(alertsFilePath);

                _alerts.AddRange(fileAlerts);
            }
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

        public string OptionsFilePath
        {
            get
            {
                return _optionsFilePath;
            }
        }

        public string AlertsFilePath
        {
            get
            {
                return _alertsFilePath;
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

        public void Run(string optionsFilePath)
        {
            Models.OptionsModel options;

            _optionsFilePath = optionsFilePath;

            if (File.Exists(_optionsFilePath))
            {
                options = GetOptions(_optionsFilePath);
            }
            else
            {
                options = ViewModels.OptionsBaseViewModel.GetDefaultOptions();

                SaveOptions(_optionsFilePath, options);
            }

            Run(options);
        }

        public void SaveOptions(string path, Models.OptionsModel options)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (TextWriter writer = new StreamWriter(fileStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Models.OptionsModel));

                    serializer.Serialize(writer, options);
                }
            }
        }

        public Models.OptionsModel GetOptions(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The options file doesn't exist on provided path: " + path);
            }

            Models.OptionsModel result;

            using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Models.OptionsModel));

                    result = serializer.Deserialize(reader) as Models.OptionsModel;
                }
            }

            return result;
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

        public void AddAlertToFile(Models.AlertModel alert, FileMode mode = FileMode.Append)
        {
            if (!File.Exists(AlertsFilePath))
            {
                mode = FileMode.Create;
            }

            using (FileStream fileStream = File.Open(AlertsFilePath, mode, FileAccess.Write, FileShare.ReadWrite))
            {
                using (TextWriter writer = new StreamWriter(fileStream))
                {
                    CsvWriter csvWriter = new CsvWriter(writer);

                    csvWriter.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                    csvWriter.Configuration.HasHeaderRecord = false;

                    csvWriter.WriteRecord(alert);

                    csvWriter.NextRecord();
                }
            }
        }

        public void AddAlert(Models.AlertModel alert)
        {
            AddAlertToFile(alert);

            _alerts.Add(alert);

            InvokeAlertAddedEvent(alert);
        }

        public void AddAlertRangeToFile(IEnumerable<Models.AlertModel> alerts, FileMode mode = FileMode.Append)
        {
            if (!File.Exists(AlertsFilePath))
            {
                mode = FileMode.Create;
            }

            using (FileStream fileStream = File.Open(AlertsFilePath, mode, FileAccess.Write, FileShare.ReadWrite))
            {
                using (TextWriter writer = new StreamWriter(fileStream))
                {
                    CsvWriter csvWriter = new CsvWriter(writer);

                    csvWriter.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                    csvWriter.Configuration.HasHeaderRecord = false;

                    csvWriter.WriteRecords(alerts);
                }
            }
        }

        public void AddAlertRange(IEnumerable<Models.AlertModel> alerts)
        {
            AddAlertRangeToFile(alerts);

            _alerts.AddRange(alerts);

            alerts.ToList().ForEach(alert => InvokeAlertAddedEvent(alert));
        }

        public List<Models.AlertModel> GetAlertsFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The alerts file doesn't exist on provided path: " + path);
            }

            List<Models.AlertModel> result = new List<Models.AlertModel>();

            using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    CsvReader csvReader = new CsvReader(reader);

                    csvReader.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                    csvReader.Configuration.HasHeaderRecord = false;

                    result = csvReader.GetRecords<Models.AlertModel>().ToList();
                }
            }

            return result;
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
            if (!string.IsNullOrEmpty(_optionsFilePath))
            {
                SaveOptions(_optionsFilePath, options);
            }
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

        private void TelegramOptionsChangedEvent_Handler(Models.TelegramOptionsModel options)
        {
        }

        public void AlertRemovedEvent_Handler(Models.AlertModel alert)
        {
            if (_alerts.Contains(alert))
            {
                _alerts.Remove(alert);

                AddAlertRangeToFile(_alerts, FileMode.Create);
            }
        }

        public void InvokeAlertAddedEvent(Models.AlertModel alert)
        {
            if (ShellView != null)
            {
                ShellView.Dispatcher.Invoke(() =>
                {
                    EventAggregator.GetEvent<Events.AlertAddedEvent>().Publish(alert);
                });
            }
            else
            {
                EventAggregator.GetEvent<Events.AlertAddedEvent>().Publish(alert);
            }
        }

        #endregion Methods
    }
}