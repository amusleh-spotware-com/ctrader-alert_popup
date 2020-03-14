using cAlgo.API.Alert.Models;
using cAlgo.API.Alert.UI.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cAlgo.API.Alert.Helpers
{
    internal static class AlertManager
    {
        #region Methods

        public static async Task<List<AlertModel>> GetAlerts()
        {
            if (!File.Exists(Configuration.Current.AlertFilePath))
            {
                return new List<AlertModel>();
            }

            return await Task.Run(async () =>
            {
                try
                {
                    var stream = await GetStream();

                    using (LiteDatabase database = new LiteDatabase(stream))
                    {
                        var collection = database.GetCollection<AlertModel>();

                        return collection.FindAll().ToList();
                    }
                }
                catch (Exception ex)
                {
                    if (ex is KeyNotFoundException || ex is InvalidCastException)
                    {
                        DeleteDatabaseFile();

                        return new List<AlertModel>();
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }

        public static async Task AddAlert(AlertModel alert)
        {
            await AddAlerts(new List<AlertModel>() { alert });
        }

        public static async Task AddAlerts(IEnumerable<AlertModel> alerts)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var stream = await GetStream();

                    using (LiteDatabase database = new LiteDatabase(stream))
                    {
                        var collection = database.GetCollection<AlertModel>();

                        collection.InsertBulk(alerts);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is KeyNotFoundException || ex is InvalidCastException)
                    {
                        DeleteDatabaseFile();
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }

        public static async Task RemoveAlert(AlertModel alert)
        {
            await RemoveAlerts(new List<AlertModel>() { alert });
        }

        public static async Task RemoveAlerts(IEnumerable<AlertModel> alerts, int tryNumber = 1)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var stream = await GetStream();

                    using (LiteDatabase database = new LiteDatabase(stream))
                    {
                        var collection = database.GetCollection<AlertModel>();

                        collection.DeleteMany(iAlert => alerts.Contains(iAlert));
                    }
                }
                catch (Exception ex)
                {
                    if (ex is KeyNotFoundException || ex is InvalidCastException)
                    {
                        DeleteDatabaseFile();
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }

        private static async Task<FileStream> GetStream(int tryNumber = 1)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    return File.Open(Configuration.Current.AlertFilePath, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
                catch (IOException)
                {
                    if (tryNumber < 5)
                    {
                        tryNumber++;

                        await Task.Delay(TimeSpan.FromSeconds(1));

                        return await GetStream(tryNumber);
                    }

                    throw;
                }
            });
        }

        private static void DeleteDatabaseFile()
        {
            try
            {
                File.Delete(Configuration.Current.AlertFilePath);
            }
            catch (IOException)
            {
            }
        }

        #endregion Methods
    }
}