using cAlgo.API.Alert.Models;
using cAlgo.API.Alert.UI.Models;
using LiteDB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace cAlgo.API.Alert
{
    internal static class DataManager
    {
        public static IEnumerable<AlertModel> GetAlerts()
        {
            var result = new List<AlertModel>();

            if (!File.Exists(Configuration.Current.AlertsFilePath))
            {
                return result;
            }

            using (var eventWaitHandle = GetWaitHandle())
            {
                eventWaitHandle.WaitOne();

                using (LiteDatabase database = new LiteDatabase(GetConnectionString()))
                {
                    var collection = database.GetCollection<AlertModel>();

                    result = collection.FindAll().ToList();
                }

                eventWaitHandle.Set();
            }

            return result;
        }

        public static void AddAlerts(params AlertModel[] alerts)
        {
            using (var eventWaitHandle = GetWaitHandle())
            {
                eventWaitHandle.WaitOne();

                using (LiteDatabase database = new LiteDatabase(GetConnectionString()))
                {
                    var collection = database.GetCollection<AlertModel>();

                    collection.InsertBulk(alerts);
                }

                eventWaitHandle.Set();
            }
        }

        public static void RemoveAlerts(params AlertModel[] alerts)
        {
            using (var eventWaitHandle = GetWaitHandle())
            {
                eventWaitHandle.WaitOne();

                using (LiteDatabase database = new LiteDatabase(GetConnectionString()))
                {
                    var collection = database.GetCollection<AlertModel>();

                    collection.DeleteMany(iAlert => alerts.Contains(iAlert));
                }

                eventWaitHandle.Set();
            }
        }

        private static string GetConnectionString()
        {
            return string.Format("Filename={0};Connection=shared", Configuration.Current.AlertsFilePath);
        }

        private static EventWaitHandle GetWaitHandle()
        {
            var waitHandleName = Configuration.Current.AlertsFilePath.Replace(Path.DirectorySeparatorChar, '_');

            return new EventWaitHandle(true, EventResetMode.AutoReset, waitHandleName);
        }
    }
}