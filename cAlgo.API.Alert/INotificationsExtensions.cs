using cAlgo.API.Internals;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace cAlgo.API.Alert
{
    public static class INotificationsExtensions
    {
        #region Fields

        private static MainWindow _window;

        private static int? lastTriggeredBar = null;

        private static Algo _algo;

        #endregion Fields

        #region Properties

        public static MainWindow Window
        {
            get
            {
                return _window;
            }
        }

        public static string DirectoryPath
        {
            get
            {
                string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cAlgo");

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                return dirPath;
            }
        }

        public static string FilePath
        {
            get
            {
                return Path.Combine(DirectoryPath, "alerts.csv");
            }
        }

        public static bool IsSoundAlertEnabled
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsSoundAlertEnabled", false));
            }
        }

        public static string SoundFilePath
        {
            get
            {
                return Registry.GetValue("SoundFilePath", string.Empty);
            }
        }

        public static bool IsEmailAlertEnabled
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsEmailAlertEnabled", false));
            }
        }

        public static int MaximumAlertsNumberToShow
        {
            get
            {
                return int.Parse(Registry.GetValue("MaximumAlertsNumberToShow", 50), CultureInfo.InvariantCulture);
            }
        }

        public static string FromEmail
        {
            get
            {
                return Registry.GetValue("FromEmail", "");
            }
        }

        public static string ToEmail
        {
            get
            {
                return Registry.GetValue("ToEmail", string.Empty);
            }
        }

        public static string CurrentTheme
        {
            get
            {
                return Registry.GetValue("CurrentTheme", "BaseDark");
            }
            set
            {
                Registry.SetValue("CurrentTheme", value);
            }
        }

        public static string CurrentAccent
        {
            get
            {
                return Registry.GetValue("CurrentAccent", "Blue");
            }
            set
            {
                Registry.SetValue("CurrentAccent", value);
            }
        }

        public static TimeZoneInfo CurrentTimeZone
        {
            get
            {
                string timeZone = Registry.GetValue("CurrentTimeZone", string.Empty);

                return !string.IsNullOrEmpty(timeZone) ?
                    TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(tz => tz.DisplayName.Equals(timeZone, StringComparison.InvariantCultureIgnoreCase)) :
                    TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(
                        tz => tz.BaseUtcOffset == DateTimeOffset.Now.Offset);
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

        public static void ShowPopup(
            this INotifications notifications,
            Algo algo,
            TradeType tradeType,
            Symbol symbol,
            TimeFrame timeFrame,
            DateTimeOffset time,
            string comment,
            TriggerType type = TriggerType.PerBar)
        {
            _algo = algo;

            if (type == TriggerType.PerBar)
            {
                int index = _algo.MarketSeries.Close.Count - 1;

                if (lastTriggeredBar.HasValue && lastTriggeredBar == index)
                {
                    return;
                }
                else
                {
                    lastTriggeredBar = index;
                }
            }

            if (_algo.GetType().BaseType == typeof(Indicator) && !(_algo as Indicator).IsLastBar)
            {
                return;
            }

            Registry.CreateKey("cTrader Alert");

            Alert alert = new Alert() { TradeSide = tradeType.ToString(), Symbol = symbol.Code, TimeFrame = timeFrame.ToString(), Time = time, Comment = comment };

            WriteAlert(alert);

            if (IsSoundAlertEnabled)
            {
                notifications.PlaySound(SoundFilePath);
            }

            if (IsEmailAlertEnabled)
            {
                string emailSubject = string.Format("{0} {1} | Trade Alert", alert.TradeSide, alert.Symbol);

                string emailBody = string.Format("An alert triggered at {0} to {1} {2} on {3} time frame with this comment: {4}", alert.Time, alert.TradeSide, alert.Symbol, alert.TimeFrame, alert.Comment);

                notifications.SendEmail(FromEmail, ToEmail, emailSubject, emailBody);
            }

            CloseWindow();

            ShowWindow();
        }

        public static void LogException(Exception ex)
        {
            _algo.Print("Exception: {0}", ex.Message);
            _algo.Print("Source: {0}", ex.Source);
            _algo.Print("StackTrace: {0}", ex.StackTrace);
            _algo.Print("Source: {0}", ex.Source);

            if (ex.InnerException != null)
            {
                _algo.Print("Source: {0}", ex.Source);

                LogException(ex.InnerException);
            }
        }

        public static void WriteAlert(Alert alert)
        {
            WriteAlerts(new List<Alert>() { alert });
        }

        public static void WriteAlerts(IEnumerable<Alert> alerts, FileMode mode = FileMode.Append)
        {
            try
            {
                using (FileStream fileStream = File.Open(FilePath, mode, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (TextWriter writer = new StreamWriter(fileStream))
                    {
                        CsvWriter csvWriter = new CsvWriter(writer);

                        csvWriter.Configuration.HasHeaderRecord = false;

                        csvWriter.WriteRecords(alerts);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is WriterException)
                {
                    File.Delete(INotificationsExtensions.FilePath);

                    _algo.Print("Alerts file cleaned due to an exception in writing");
                }

                INotificationsExtensions.LogException(ex);
            }
        }

        public static List<Alert> ReadAlerts()
        {
            List<Alert> alerts = new List<Alert>();

            try
            {
                using (FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (TextReader reader = new StreamReader(fileStream))
                    {
                        CsvReader csvReader = new CsvReader(reader);

                        csvReader.Configuration.HasHeaderRecord = false;

                        alerts = csvReader.GetRecords<Alert>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is ReaderException)
                {
                    File.Delete(INotificationsExtensions.FilePath);

                    _algo.Print("Alerts file cleaned due to an exception in reading");
                }

                INotificationsExtensions.LogException(ex);
            }

            return alerts;
        }

        public static void ShowWindow()
        {
            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    _window = new MainWindow();

                    _window.ShowDialog();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }));

            windowThread.SetApartmentState(ApartmentState.STA);

            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();
        }

        public static void CloseWindow()
        {
            if (Window != null && !Window.IsClosed)
            {
                Window.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Window?.Close();
                    }
                    catch (Exception ex)
                    {
                        INotificationsExtensions.LogException(ex);
                    }
                });
            }
        }

        public static ResourceDictionary GetStyleResource(string name)
        {
            Uri uri = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", name));

            return new ResourceDictionary() { Source = uri };
        }

        #endregion Methods
    }
}