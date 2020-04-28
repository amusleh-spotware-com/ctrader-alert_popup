using cAlgo.API.Alert.Models;
using cAlgo.API.Alert.UI.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cAlgo.API.Alert.Helpers
{
    internal static class AlertManager
    {
        private static object _locker = new object();

        public static IEnumerable<AlertModel> GetAlerts()
        {
            if (!File.Exists(Configuration.Current.AlertsFilePath))
            {
                return new List<AlertModel>();
            }

            var alertsFileCopy = Configuration.Current.GetAlertsFileCopy();

            try
            {
                var connectionString = GetConnectionString(alertsFileCopy.FullName);

                using (LiteDatabase database = new LiteDatabase(connectionString))
                {
                    var collection = database.GetCollection<AlertModel>();

                    return collection.FindAll().ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                File.Delete(alertsFileCopy.FullName);
            }
        }

        public static void AddAlerts(params AlertModel[] alerts)
        {
            var connectionString = GetConnectionString(Configuration.Current.AlertsFilePath);

            lock (_locker)
            {
                using (LiteDatabase database = new LiteDatabase(connectionString))
                {
                    var collection = database.GetCollection<AlertModel>();

                    collection.InsertBulk(alerts);
                }
            }
        }

        public static void RemoveAlerts(params AlertModel[] alerts)
        {
            var connectionString = GetConnectionString(Configuration.Current.AlertsFilePath);

            lock (_locker)
            {
                using (LiteDatabase database = new LiteDatabase(connectionString))
                {
                    var collection = database.GetCollection<AlertModel>();

                    collection.DeleteMany(iAlert => alerts.Contains(iAlert));
                }
            }
        }

        private static string GetConnectionString(string fileNmae)
        {
            return string.Format("Filename={0};Connection=shared", fileNmae);
        }
    }
}