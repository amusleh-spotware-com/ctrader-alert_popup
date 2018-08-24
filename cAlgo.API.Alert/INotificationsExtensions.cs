using cAlgo.API.Internals;
using System;
using cAlgo.API.Alert.UI;
using cAlgo.API.Alert.UI.Models;
using System.Threading;
using System.Globalization;
using System.IO;

namespace cAlgo.API.Alert
{
    public static class INotificationsExtensions
    {
        #region Methods

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, Types.TradeSide tradeSide)
        {
            ShowPopup(notifications, timeFrame, symbol, "Unknown", tradeSide);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, string triggeredBy, Types.TradeSide tradeSide)
        {
            ShowPopup(notifications, timeFrame, symbol, triggeredBy, tradeSide, string.Empty);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, string triggeredBy, Types.TradeSide tradeSide, string comment)
        {
            ShowPopup(notifications, timeFrame, symbol, triggeredBy, tradeSide, comment, DateTimeOffset.Now);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, string triggeredBy, Types.TradeSide tradeSide, string comment, DateTimeOffset time)
        {
            ShowPopup(notifications, timeFrame.ToString(), symbol.ToString(), triggeredBy, tradeSide.ToString(), comment, time);
        }

        public static void ShowPopup(this INotifications notifications, string timeFrame, string symbol, string triggeredBy, string tradeSide, string comment, DateTimeOffset time)
        {
            AlertModel alert = new AlertModel
            {
                TimeFrame = timeFrame,
                Symbol = symbol,
                TriggeredBy = triggeredBy,
                TradeSide = tradeSide,
                Comment = comment,
                Time = time
            };

            Controller.SetupConfigurationPaths();

            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Bootstrapper bootstrapper = new Bootstrapper(Configuration.AlertFilePath, Configuration.OptionsFilePath);

                    Controller.TriggerAlerts(notifications, bootstrapper.Options, alert);

                    bootstrapper.AddAlert(alert);

                    bootstrapper.Run();
                }
                catch (Exception ex)
                {
                    Controller.LogException(ex);
                }
            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();
        }

        #endregion Methods
    }
}