using cAlgo.API.Alert.Models;
using cAlgo.API.Alert.UI.Models;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cAlgo.API.Alert.Helpers
{
    internal static class AlertManager
    {
        public static string ConnectionString
        {
            get
            {
                return string.Format("Filename={0};Connection=shared", Configuration.Current.AlertFilePath);
            }
        }

        public static List<AlertModel> GetAlerts()
        {
            if (!File.Exists(Configuration.Current.AlertFilePath))
            {
                return new List<AlertModel>();
            }

            using (LiteDatabase database = new LiteDatabase(ConnectionString))
            {
                var collection = database.GetCollection<AlertModel>();

                return collection.FindAll().ToList();
            }
        }

        public static void AddAlert(AlertModel alert)
        {
            AddAlerts(new List<AlertModel>() { alert });
        }

        public static void AddAlerts(IEnumerable<AlertModel> alerts)
        {
            using (LiteDatabase database = new LiteDatabase(ConnectionString))
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
            using (LiteDatabase database = new LiteDatabase(ConnectionString))
            {
                var collection = database.GetCollection<AlertModel>();

                collection.DeleteMany(iAlert => alerts.Contains(iAlert));
            }
        }
    }
}