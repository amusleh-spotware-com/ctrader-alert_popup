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
        public static string ConnectionString => string.Format("Filename={0};Connection=shared", Configuration.Current.AlertFilePath);

        public static IEnumerable<AlertModel> GetAlerts()
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

        public static void AddAlerts(params AlertModel[] alerts)
        {
            using (LiteDatabase database = new LiteDatabase(ConnectionString))
            {
                var collection = database.GetCollection<AlertModel>();

                collection.InsertBulk(alerts);
            }
        }

        public static void RemoveAlerts(params AlertModel[] alerts)
        {
            using (LiteDatabase database = new LiteDatabase(ConnectionString))
            {
                var collection = database.GetCollection<AlertModel>();

                collection.DeleteMany(iAlert => alerts.Contains(iAlert));
            }
        }
    }
}