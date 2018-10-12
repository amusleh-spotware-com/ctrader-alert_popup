using cAlgo.API.Alert.Types;
using cAlgo.API.Alert.UI.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Alert
{
    public static class INotificationsExtensions
    {
        #region Methods

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, TradeType tradeType)
        {
            ShowPopup(notifications, timeFrame, symbol, "Unknown", tradeType);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, string triggeredBy, TradeType tradeType)
        {
            ShowPopup(notifications, timeFrame, symbol, symbol.Bid, triggeredBy, tradeType, string.Empty);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, double price, string triggeredBy, TradeType tradeType, string comment)
        {
            ShowPopup(notifications, timeFrame, symbol, price, triggeredBy, tradeType, comment, DateTimeOffset.Now);
        }

        public static void ShowPopup(this INotifications notifications, TimeFrame timeFrame, Symbol symbol, double price, string triggeredBy, TradeType tradeType, string comment, DateTimeOffset time)
        {
            ShowPopup(notifications, timeFrame.ToString(), symbol.Code.ToString(), price, triggeredBy, tradeType.ToString(), comment, time);
        }

        public static void ShowPopup(this INotifications notifications, string timeFrame, string symbol, double price, string triggeredBy, string tradeSide, string comment, DateTimeOffset time)
        {
            AlertModel alert = new AlertModel
            {
                TimeFrame = timeFrame,
                Symbol = symbol,
                Price = price,
                TriggeredBy = triggeredBy,
                TradeSide = tradeSide,
                Comment = comment,
                Time = time
            };

            Controller.SetConfigurationIfNotYet();

            Controller.Show(notifications, alert);
        }

        #endregion Methods
    }
}