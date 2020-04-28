using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace cAlgo.API.Alert.Models
{
    public class Configuration
    {
        public Configuration()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            string alertsDirPath = Path.Combine(documentsPath, "cAlgo", "Alerts", version.ToString());

            if (!Directory.Exists(alertsDirPath))
            {
                Directory.CreateDirectory(alertsDirPath);
            }

            AlertsFilePath = Path.Combine(alertsDirPath, $"Alerts_{version}.db");
            SettingsFilePath = Path.Combine(alertsDirPath, $"AlertPopupSettings_{version}.xml");
            LogFilePath = Path.Combine(alertsDirPath, $"Alerts_{version}.log");

            Title = $"Alerts - cTrader";
        }

        #region Properties

        public string AlertsFilePath { get; set; }

        public string SettingsFilePath { get; set; }

        public string LogFilePath { get; set; }

        public string Title { get; set; }

        public static Configuration Current { get; set; } = new Configuration();

        #endregion Properties

        #region Methods

        public FileInfo GetAlertsFileCopy()
        {
            if (!File.Exists(AlertsFilePath))
            {
                throw new FileNotFoundException("Couldn't find the alerts file to copy: " + AlertsFilePath);
            }

            var alertsFileInfo = new FileInfo(AlertsFilePath);

            var alertFileNameWithoutExtension = alertsFileInfo.Name.Substring(0, alertsFileInfo.Name.Length - 4);

            var copyFileName = string.Format("{0}_{1}_{2}_{3}.db", alertFileNameWithoutExtension, DateTime.Now.Ticks,
                Thread.CurrentThread.ManagedThreadId, Assembly.GetExecutingAssembly().FullName);

            var copyFilePath = Path.Combine(alertsFileInfo.DirectoryName, copyFileName);

            File.Copy(alertsFileInfo.FullName, copyFilePath);

            return new FileInfo(copyFilePath);
        }

        #endregion
    }
}