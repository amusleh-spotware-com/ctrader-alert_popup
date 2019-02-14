using System;
using System.IO;
using System.Reflection;

namespace cAlgo.API.Alert.Models
{
    public class Configuration
    {
        public Configuration()
        {
            Tracer = new Action<string>(message => System.Diagnostics.Trace.WriteLine(message));

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string calgoDirPath = Path.Combine(documentsPath, "cAlgo");

            if (!Directory.Exists(calgoDirPath))
            {
                Directory.CreateDirectory(calgoDirPath);
            }

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            AlertFilePath = Path.Combine(calgoDirPath, $"Alerts_{version}.db");
            SettingsFilePath = Path.Combine(calgoDirPath, $"AlertPopupSettings_{version}.xml");

            Title = $"Alerts - cTrader {version}";
        }

        #region Properties

        public string AlertFilePath { get; set; }

        public string SettingsFilePath { get; set; }

        public Action<string> Tracer { get; set; }

        public string Title { get; set; }

        public static Configuration Current { get; set; } = new Configuration();

        #endregion Properties
    }
}