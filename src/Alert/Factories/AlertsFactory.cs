using cAlgo.API.Alert.Models;
using cAlgo.API.Alert.UI.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace cAlgo.API.Alert.Factories
{
    internal static class AlertsFactory
    {
        #region Methods

        public static IEnumerable<AlertModel> GetAlerts()
        {
            if (!File.Exists(Configuration.Current.AlertFilePath))
            {
                return new List<AlertModel>();
            }

            using (LiteDatabase database = new LiteDatabase(GetStream()))
            {
                var collection = database.GetCollection<AlertModel>();

                return collection.FindAll();
            }
        }

        public static void AddAlert(AlertModel alert)
        {
            AddAlerts(new List<AlertModel>() { alert });
        }

        public static void AddAlerts(IEnumerable<AlertModel> alerts)
        {
            using (LiteDatabase database = new LiteDatabase(GetStream()))
            {
                var collection = database.GetCollection<AlertModel>();

                collection.InsertBulk(alerts);
            }
        }

        public static void RemoveAlert(AlertModel alert)
        {
            RemoveAlerts(new List<AlertModel>() { alert });
        }

        public static void RemoveAlerts(IEnumerable<AlertModel> alerts)
        {
            using (LiteDatabase database = new LiteDatabase(GetStream()))
            {
                var collection = database.GetCollection<AlertModel>();

                collection.Delete(iAlert => alerts.Contains(iAlert));
            }
        }

        private static FileStream GetStream(int maxTry = 5)
        {
            FileStream stream = null;

            try
            {
                maxTry--;

                if (maxTry > 0)
                {
                    stream = File.Open(Configuration.Current.AlertFilePath, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
            }
            catch (IOException)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));

                return GetStream(maxTry);
            }

            if (stream == null)
            {
                throw new NullReferenceException("Couldn't get the file stream");
            }

            return stream;
        }

        #endregion Methods
    }
}