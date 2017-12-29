namespace Alert
{
    using cAlgo.API;
    using cAlgo.API.Internals;
    using System;
    using System.Threading;
    using System.IO;
    using Nortal.Utilities.Csv;
    using System.Globalization;
    using MahApps.Metro;
    using System.Linq;

    public class Manager
    {
        #region Fields
        private static AlertWindow window;
        #endregion

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

        public static bool IsWindowOpen
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsOpen", false));
            }
            set
            {
                Registry.SetValue("IsOpen", value);
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
        #endregion

        #region Methods
        public static void Trigger(TradeType tradeType, Symbol symbol, TimeFrame timeFrame, DateTime time, string comment)
        {
            Registry.CreateKey("cTrader Alert");

            if (!Directory.Exists(Manager.DirectoryPath))
            {
                Directory.CreateDirectory(Manager.DirectoryPath);
            }

            if (!File.Exists(Manager.FilePath))
            {
                File.Create(Manager.FilePath).Close();
            }

            Alert alert = new Alert() { TradeSide = tradeType.ToString(), Symbol = symbol.Code, TimeFrame = timeFrame.ToString(), Time = time, Comment = comment };

            using (StringWriter writer = new StringWriter())
            {
                CsvWriter csv = new CsvWriter(writer, new CsvSettings());

                csv.WriteLine(alert.TradeSide, alert.Symbol, alert.TimeFrame, alert.Time.ToString(), alert.Comment);

                File.AppendAllText(FilePath, writer.ToString());
            }

            if (!IsWindowOpen)
            {
                Thread alertWindowThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                        window = new AlertWindow();

                        IsWindowOpen = true;

                        window.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                }));

                alertWindowThread.SetApartmentState(ApartmentState.STA);

                alertWindowThread.Start();
            }
            else
            {
                Registry.SetValue("Refresh", true);
            }

            if (IsSoundAlertEnabled)
            {
                if (Manager.Robot != null)
                {
                    Manager.Robot.Notifications.PlaySound(SoundFilePath);
                }
                else if (Manager.Indicator != null)
                {
                    Manager.Indicator.Notifications.PlaySound(SoundFilePath);
                }
            }

            if (IsEmailAlertEnabled)
            {
                string emailSubject = string.Format("{0} {1} | Trade Alert", alert.TradeSide, alert.Symbol);

                string emailBody = string.Format("An alert triggered at {0} to {1} {2} on {3} time frame with this comment: {4}", alert.Time, alert.TradeSide, alert.Symbol, alert.TimeFrame, alert.Comment);

                if (Manager.Robot != null)
                {
                    Manager.Robot.Notifications.SendEmail(FromEmail, ToEmail, emailSubject, emailBody);
                }
                else if (Manager.Indicator != null)
                {
                    Manager.Indicator.Notifications.SendEmail(FromEmail, ToEmail, emailSubject, emailBody);
                }
            }
        }

        public static void Print(object obj)
        {
            if (Manager.Robot != null)
            {
                Manager.Robot.Print(obj);
            }
            else if (Manager.Indicator != null)
            {
                Manager.Indicator.Print(obj);
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
        #endregion
    }
}
