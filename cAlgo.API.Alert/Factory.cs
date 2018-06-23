using cAlgo.API;
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
    public static class Factory
    {
        #region Fields

        private static MainWindow _window;

        private static int? lastTriggeredBar = null;

        #endregion Fields

        #region Properties

        public static MainWindow Window
        {
            get
            {
                return _window;
            }
        }

        public static Algo Algo { get; set; }

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

        public static int Index
        {
            get
            {
                return Algo.MarketSeries.Close.Count - 1;
            }
        }

        #endregion Properties

        #region Methods

        public static void Trigger(
            TradeType tradeType,
            Symbol symbol,
            TimeFrame timeFrame,
            DateTimeOffset time,
            string comment,
            TriggerType type = TriggerType.PerBar)
        {
            if (type == TriggerType.PerBar)
            {
                if (lastTriggeredBar.HasValue && lastTriggeredBar == Index)
                {
                    return;
                }
                else
                {
                    lastTriggeredBar = Index;
                }
            }

            if (Algo.GetType().BaseType == typeof(Indicator) && !(Algo as Indicator).IsLastBar)
            {
                return;
            }

            Registry.CreateKey("cTrader Alert");

            Alert alert = new Alert() { TradeSide = tradeType.ToString(), Symbol = symbol.Code, TimeFrame = timeFrame.ToString(), Time = time, Comment = comment };

            WriteAlert(alert);

            if (IsSoundAlertEnabled)
            {
                PlaySound(SoundFilePath);
            }

            if (IsEmailAlertEnabled)
            {
                string emailSubject = string.Format("{0} {1} | Trade Alert", alert.TradeSide, alert.Symbol);

                string emailBody = string.Format("An alert triggered at {0} to {1} {2} on {3} time frame with this comment: {4}", alert.Time, alert.TradeSide, alert.Symbol, alert.TimeFrame, alert.Comment);

                SendEmail(FromEmail, ToEmail, emailSubject, emailBody);
            }

            CloseWindow();

            ShowWindow();
        }

        public static void Print(object obj)
        {
            Factory.Algo.Print(obj);
        }

        public static void PlaySound(string soundFilePath)
        {
            Factory.Algo.Notifications.PlaySound(soundFilePath);
        }

        public static void SendEmail(string fromEmail, string toEmail, string emailSubject, string emailBody)
        {
            Factory.Algo.Notifications.SendEmail(fromEmail, toEmail, emailSubject, emailBody);
        }

        public static void LogException(Exception ex)
        {
            Print(string.Format("Exception: {0}", ex.Message));
            Print(string.Format("Source: {0}", ex.Source));
            Print(string.Format("StackTrace: {0}", ex.StackTrace));
            Print(string.Format("TargetSite: {0}", ex.TargetSite));

            if (ex.InnerException != null)
            {
                Print(string.Format("InnerException: {0}", ex.InnerException));

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
                    File.Delete(Factory.FilePath);

                    Print("Alerts file cleaned due to an exception in writing");
                }

                Factory.LogException(ex);
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
                    File.Delete(Factory.FilePath);

                    Print("Alerts file cleaned due to an exception in reading");
                }

                Factory.LogException(ex);
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
                        Factory.LogException(ex);
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