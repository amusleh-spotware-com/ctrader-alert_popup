using CsvHelper;
using MahApps.Metro;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI
{
    public class Bootstrapper
    {
        #region Fields

        private readonly List<Models.AlertModel> _alerts;
        private readonly string _alertsFilePath;
        private readonly EventAggregator _eventAggregator;
        private readonly List<string> _navigationJournal;
        private readonly Views.ShellView _shellView;
        private readonly ResourceDictionary _themeResources;
        private string _currentView;
        private Models.OptionsModel _options;
        private string _optionsFilePath;

        #endregion Fields

        public Bootstrapper(string alertsFilePath, string optionsFilePath)
        {
            _alertsFilePath = alertsFilePath;

            _optionsFilePath = optionsFilePath;

            _themeResources = new ResourceDictionary();

            _shellView = CreateView<Views.ShellView, ViewModels.ShellViewModel>(this);

            if (File.Exists(_optionsFilePath))
            {
                _options = GetOptions(_optionsFilePath);
            }
            else
            {
                _options = ViewModels.OptionsBaseViewModel.GetDefaultOptions();

                SaveOptions(_optionsFilePath, _options);
            }

            _shellView.Topmost = _options.General.TopMost;

            _themeResources.MergedDictionaries.Add(_options.General.ThemeAccent.Accent.Resources);
            _themeResources.MergedDictionaries.Add(_options.General.ThemeBase.Base.Resources);

            _navigationJournal = new List<string>();

            _eventAggregator = new EventAggregator();

            _eventAggregator.GetEvent<Events.OptionsChangedEvent>().Subscribe(OptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.GeneralOptionsChangedEvent>().Subscribe(GeneralOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.AlertOptionsChangedEvent>().Subscribe(AlertOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.SoundOptionsChangedEvent>().Subscribe(SoundOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.EmailOptionsChangedEvent>().Subscribe(EmailOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.TelegramOptionsChangedEvent>().Subscribe(TelegramOptionsChangedEvent_Handler);
            _eventAggregator.GetEvent<Events.AlertRemovedEvent>().Subscribe(AlertRemovedEvent_Handler);

            _alerts = new List<Models.AlertModel>();

            if (File.Exists(_alertsFilePath))
            {
                List<Models.AlertModel> fileAlerts = GetAlertsFromFile(alertsFilePath);

                _alerts.AddRange(fileAlerts);
            }
        }

        #region Properties

        public string AlertsFilePath
        {
            get
            {
                return _alertsFilePath;
            }
        }

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

        public Models.OptionsModel Options
        {
            get
            {
                return _options;
            }
        }

        public string OptionsFilePath
        {
            get
            {
                return _optionsFilePath;
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

        #endregion Properties

        #region Methods

        public static Models.OptionsModel GetOptions(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The options file doesn't exist on provided path: " + path);
            }

            Models.OptionsModel result;

            using (FileStream fileStream = GetStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Models.OptionsModel));

                    try
                    {
                        result = serializer.Deserialize(reader) as Models.OptionsModel;
                    }
                    catch (InvalidOperationException ex)
                    {
                        fileStream.Close();

                        File.Delete(path);

                        throw ex;
                    }
                }
            }

            return result;
        }

        public static void SaveOptions(string path, Models.OptionsModel options)
        {
            using (FileStream fileStream = GetStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (TextWriter writer = new StreamWriter(fileStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Models.OptionsModel));

                    serializer.Serialize(writer, options);
                }
            }
        }

        public void AddAlert(Models.AlertModel alert)
        {
            AddAlertToFile(alert);

            _alerts.Add(alert);

            InvokeAlertAddedEvent(alert);
        }

        public void AddAlertRange(IEnumerable<Models.AlertModel> alerts)
        {
            AddAlertRangeToFile(alerts);

            _alerts.AddRange(alerts);

            alerts.ToList().ForEach(alert => InvokeAlertAddedEvent(alert));
        }

        public void AddAlertRangeToFile(IEnumerable<Models.AlertModel> alerts, FileMode mode = FileMode.Append)
        {
            if (!File.Exists(AlertsFilePath))
            {
                mode = FileMode.Create;
            }

            using (FileStream fileStream = GetStream(AlertsFilePath, mode, FileAccess.Write, FileShare.ReadWrite))
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

        public void AddAlertToFile(Models.AlertModel alert, FileMode mode = FileMode.Append)
        {
            if (!File.Exists(AlertsFilePath))
            {
                mode = FileMode.Create;
            }

            using (FileStream fileStream = GetStream(AlertsFilePath, mode, FileAccess.Write, FileShare.ReadWrite))
            {
                using (TextWriter writer = new StreamWriter(fileStream))
                {
                    CsvWriter csvWriter = new CsvWriter(writer);

                    csvWriter.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                    csvWriter.Configuration.HasHeaderRecord = false;
                    csvWriter.Configuration.RegisterClassMap<Types.AlertCsvMap>();

                    csvWriter.WriteRecord(alert);

                    csvWriter.NextRecord();
                }
            }
        }

        public void AlertRemovedEvent_Handler(Models.AlertModel alert)
        {
            if (_alerts.Contains(alert))
            {
                _alerts.Remove(alert);

                AddAlertRangeToFile(_alerts, FileMode.Create);
            }
        }

        public List<Models.AlertModel> GetAlerts()
        {
            return _alerts.ToList();
        }

        public List<Models.AlertModel> GetAlertsFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The alerts file doesn't exist on provided path: " + path);
            }

            List<Models.AlertModel> result = new List<Models.AlertModel>();

            using (FileStream fileStream = GetStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    CsvReader csvReader = new CsvReader(reader);

                    csvReader.Configuration.CultureInfo = CultureInfo.InvariantCulture;
                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.RegisterClassMap<Types.AlertCsvMap>();

                    try
                    {
                        result = csvReader.GetRecords<Models.AlertModel>().ToList();
                    }
                    catch (CsvHelperException ex)
                    {
                        fileStream.Close();

                        File.Delete(path);

                        throw ex;
                    }
                }
            }

            return result;
        }

        public void InvokeAlertAddedEvent(Models.AlertModel alert)
        {
            if (ShellView != null)
            {
                InvokeOnWindowThread(() =>
                {
                    EventAggregator.GetEvent<Events.AlertAddedEvent>().Publish(alert);
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
                    _shellView.Content = CreateView<Views.AlertsView, ViewModels.AlertsViewModel>(_alerts, _options, _eventAggregator);
                    break;

                case ViewNames.OptionsView:
                    _shellView.Content = CreateView<Views.OptionsView, ViewModels.OptionsViewModel>(_options, _eventAggregator);
                    break;
            }
        }

        public void Run()
        {
            _shellView.ShowDialog();
        }

        public void Shutdown()
        {
            _shellView.Close();
        }

        private static FileStream GetStream(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare = FileShare.ReadWrite,
            int maxTry = 5)
        {
            FileStream stream = null;

            try
            {
                maxTry--;

                if (maxTry > 0)
                {
                    stream = File.Open(path, fileMode, fileAccess, fileShare);
                }
            }
            catch (IOException)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));

                return GetStream(path, fileMode, fileAccess, fileShare, maxTry);
            }

            if (stream == null)
            {
                throw new NullReferenceException("Couldn't get the file stream");
            }

            return stream;
        }

        private void AlertOptionsChangedEvent_Handler(Models.AlertOptionsModel options)
        {
        }

        private TView CreateView<TView, TViewModel>(params object[] parameters) where TView : class
            where TViewModel : class
        {
            TView view = (TView)Activator.CreateInstance(typeof(TView));

            (view as ContentControl).Resources.MergedDictionaries.Add(_themeResources);

            (view as FrameworkElement).DataContext = GetViewModel<TViewModel>(parameters);

            return view;
        }

        private void EmailOptionsChangedEvent_Handler(Models.EmailOptionsModel options)
        {
        }

        private void GeneralOptionsChangedEvent_Handler(Models.GeneralOptionsModel options)
        {
            _shellView.Topmost = options.TopMost;

            ThemeManager.ChangeAppStyle(_themeResources, options.ThemeAccent.Accent, options.ThemeBase.Base);
        }

        private T GetViewModel<T>(params object[] parameters) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        private void OptionsChangedEvent_Handler(Models.OptionsModel options)
        {
            if (!string.IsNullOrEmpty(_optionsFilePath))
            {
                SaveOptions(_optionsFilePath, options);
            }
        }

        private void SoundOptionsChangedEvent_Handler(Models.SoundOptionsModel options)
        {
        }

        private void TelegramOptionsChangedEvent_Handler(Models.TelegramOptionsModel options)
        {
        }

        #endregion Methods
    }
}