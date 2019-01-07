using cAlgo.API.Alert.Types;
using cAlgo.API.Alert.UI.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Alert
{
    public static class INotificationsExtensions
    {
        #region Methods

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, TradeType type)
        {
            ShowPopup(notifications, timeFrame, symbol, type, "Unknown");
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, TradeType type, string triggeredBy)
        {
            ShowPopup(notifications, timeFrame, symbol, type, triggeredBy, symbol.Bid);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, TradeType type, string triggeredBy, double price)
        {
            ShowPopup(notifications, timeFrame, symbol, type, triggeredBy, price, string.Empty);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, TradeType type, string triggeredBy, double price, string comment)
        {
            ShowPopup(notifications, timeFrame, symbol, type, triggeredBy, price, comment, DateTimeOffset.Now);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, TradeType type, string triggeredBy, double price, string comment, DateTimeOffset time)
        {
            ShowPopup(notifications, timeFrame, symbol, type, triggeredBy, price, comment, DateTimeOffset.Now);
        }

        public static void ShowPopup(this INotifications notifications, string timeFrame, string symbol, string type, string triggeredBy, double price, string comment, DateTimeOffset time)
        {
            AlertModel alert = new AlertModel
            {
                TimeFrame = timeFrame,
                Symbol = symbol,
                Price = price,
                TriggeredBy = triggeredBy,
                Type = type,
                Comment = comment,
                Time = time
            };

            Controller.SetConfigurationIfNotYet();

            Controller.Show(notifications, alert);
        }

        #endregion Methods
    }
}