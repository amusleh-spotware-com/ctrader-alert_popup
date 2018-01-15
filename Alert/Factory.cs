using cAlgo.API;
using cAlgo.API.Internals;
using CsvHelper;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace Alert
{
    public class Factory
    {
        #region Fields

        private static AlertWindow window;

        private static Mutex mutex;

        #endregion Fields

        #region Properties

        public static AlertWindow Window
        {
            get
            {
                return window;
            }
        }

        public static Robot Robot { get; set; }

        public static Indicator Indicator { get; set; }

        public static string DirectoryPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cAlgo");
            }
        }

        public static string FilePath
        {
            get
            {
                return Path.Combine(DirectoryPath, "alerts.csv");
            }
        }

        public static AppTheme CurrentTheme
        {
            get
            {
                AppTheme currentTheme = ThemeManager.AppThemes.ToList().SingleOrDefault(theme => theme.Name == Registry.GetValue("CurrentTheme", string.Empty));

                if (currentTheme == null)
                {
                    currentTheme = ThemeManager.AppThemes.First();
                }

                return currentTheme;
            }
        }

        public static Accent CurrentAccent
        {
            get
            {
                Accent currentAccent = ThemeManager.Accents.ToList().SingleOrDefault(accent => accent.Name == Registry.GetValue("CurrentAccent", string.Empty));

                if (currentAccent == null)
                {
                    currentAccent = ThemeManager.Accents.First();
                }

                return currentAccent;
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

        public static Mutex Mutex
        {
            get
            {
                return mutex;
            }
        }

        #endregion Properties

        #region Methods

        public static void Trigger(TradeType tradeType, Symbol symbol, TimeFrame timeFrame, DateTime time, string comment)
        {
            Registry.CreateKey("cTrader Alert");

            if (!Directory.Exists(Factory.DirectoryPath))
            {
                Directory.CreateDirectory(Factory.DirectoryPath);
            }

            if (!File.Exists(Factory.FilePath))
            {
                File.Create(Factory.FilePath).Close();
            }

            Alert alert = new Alert() { TradeSide = tradeType.ToString(), Symbol = symbol.Code, TimeFrame = timeFrame.ToString(), Time = time, Comment = comment };

            WriteAlert(alert);

            bool isWindowNotOpen = false;

            mutex = new Mutex(true, Properties.Settings.Default.MutexName, out isWindowNotOpen);

            if (isWindowNotOpen)
            {
                ShowAlertWindow();
            }
            else
            {
                RefreshAlertWindow();
            }

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
        }

        public static void Print(object obj)
        {
            if (Factory.Robot != null)
            {
                Factory.Robot.Print(obj);
            }
            else if (Factory.Indicator != null)
            {
                Factory.Indicator.Print(obj);
            }
        }

        public static void PlaySound(string soundFilePath)
        {
            if (Factory.Robot != null)
            {
                Factory.Robot.Notifications.PlaySound(soundFilePath);
            }
            else if (Factory.Indicator != null)
            {
                Factory.Indicator.Notifications.PlaySound(soundFilePath);
            }
        }

        public static void SendEmail(string fromEmail, string toEmail, string emailSubject, string emailBody)
        {
            if (Factory.Robot != null)
            {
                Factory.Robot.Notifications.SendEmail(fromEmail, toEmail, emailSubject, emailBody);
            }
            else if (Factory.Indicator != null)
            {
                Factory.Indicator.Notifications.SendEmail(fromEmail, toEmail, emailSubject, emailBody);
            }
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
            }
        }

        public static void WriteAlert(Alert alert)
        {
            using (TextWriter writer = File.CreateText(FilePath))
            {
                CsvWriter csvWriter = new CsvWriter(writer);

                csvWriter.WriteRecords(new List<Alert>() { alert });
            }
        }

        public static void ShowAlertWindow()
        {
            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    window = new AlertWindow();

                    window.ShowDialog();
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

        public static void CloseAlertWindow()
        {
            if (window != null)
            {
                window.Invoke(() =>
                {
                    window.Close();
                });
            }
        }

        public static void RefreshAlertWindow()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(Properties.Settings.Default.PipeServerName))
            {
                pipeClient.Connect();
                pipeClient.WriteByte(1);
            }
        }

        public static void StopPipeServer()
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(Properties.Settings.Default.PipeServerName))
                {
                    pipeClient.Connect();

                    pipeClient.WriteByte(0);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        #endregion Methods
    }
}