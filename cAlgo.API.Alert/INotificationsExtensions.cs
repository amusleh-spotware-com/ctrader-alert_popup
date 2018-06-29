using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Alert
{
    public static class INotificationsExtensions
    {
        #region Fields

        private static int? lastTriggeredBar = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Shows a pop window that contains a list of alerts, it uses the current symbol, time frame and server time
        /// Use other overloads of this method if you want to have more control over inputs
        /// </summary>
        /// <param name="notifications">Algo Notifications object</param>
        /// <param name="algo">The calling algo (indicator or cBot), use "this"</param>
        /// <param name="tradeType">The alert trade type</param>
        /// <param name="comment">Any extra detail about alert</param>
        /// <param name="perBar">Only trigger alert if the bar changed?</param>
        public static void ShowAlertPopup(
            this INotifications notifications,
            Algo algo,
            TradeType tradeType,
            string comment,
            bool perBar = true)
        {
            ShowAlertPopup(notifications, algo, tradeType, algo.Symbol, algo.TimeFrame, algo.Server.Time, comment, perBar);
        }

        /// <summary>
        /// Shows a pop window that contains a list of alerts, it uses the current symbol, and time frame
        /// Use other overloads of this method if you want to have more control over inputs
        /// </summary>
        /// <param name="notifications">Algo Notifications object</param>
        /// <param name="algo">The calling algo (indicator or cBot), use "this"</param>
        /// <param name="tradeType">The alert trade type</param>
        /// <param name="time">The alert time</param>
        /// <param name="comment">Any extra detail about alert</param>
        /// <param name="perBar">Only trigger alert if the bar changed?</param>
        public static void ShowAlertPopup(
            this INotifications notifications,
            Algo algo,
            TradeType tradeType,
            DateTimeOffset time,
            string comment,
            bool perBar = true)
        {
            ShowAlertPopup(notifications, algo, tradeType, algo.Symbol, algo.TimeFrame, time, comment, perBar);
        }

        /// <summary>
        /// Shows a pop window that contains a list of alerts, it uses the server time
        /// Use other overloads of this method if you want to have more control over inputs
        /// </summary>
        /// <param name="notifications">Algo Notifications object</param>
        /// <param name="algo">The calling algo (indicator or cBot), use "this"</param>
        /// <param name="tradeType">The alert trade type</param>
        /// <param name="symbol">The symbol you are triggering alert for</param>
        /// <param name="timeFrame">Time frame of your indicator, or cBot</param>
        /// <param name="comment">Any extra detail about alert</param>
        /// <param name="perBar">Only trigger alert if the bar changed?</param>
        public static void ShowAlertPopup(
            this INotifications notifications,
            Algo algo,
            TradeType tradeType,
            Symbol symbol,
            TimeFrame timeFrame,
            string comment,
            bool perBar = true)
        {
            ShowAlertPopup(notifications, algo, tradeType, symbol, timeFrame, algo.Server.Time, comment, perBar);
        }

        /// <summary>
        /// Shows a pop window that contains a list of alerts
        /// </summary>
        /// <param name="notifications">Algo Notifications object</param>
        /// <param name="algo">The calling algo (indicator or cBot), use "this"</param>
        /// <param name="tradeType">The alert trade type</param>
        /// <param name="symbol">The symbol you are triggering alert for</param>
        /// <param name="timeFrame">Time frame of your indicator, or cBot</param>
        /// <param name="time">The alert time</param>
        /// <param name="comment">Any extra detail about alert</param>
        /// <param name="perBar">Only trigger alert if the bar changed?</param>
        public static void ShowAlertPopup(
            this INotifications notifications,
            Algo algo,
            TradeType tradeType,
            Symbol symbol,
            TimeFrame timeFrame,
            DateTimeOffset time,
            string comment,
            bool perBar = true)
        {
            Factory.Algo = algo;

            if (perBar)
            {
                int index = algo.MarketSeries.Close.Count - 1;

                if (lastTriggeredBar.HasValue && lastTriggeredBar == index)
                {
                    return;
                }
                else
                {
                    lastTriggeredBar = index;

                    ShowAlertPopup(notifications, algo, tradeType, symbol, timeFrame, time, comment);
                }
            }

            if (algo.GetType().BaseType == typeof(Indicator) && !(algo as Indicator).IsLastBar)
            {
                return;
            }

            Registry.CreateKey("cTrader Alert");

            Alert alert = new Alert()
            {
                TradeSide = tradeType.ToString(),
                Symbol = symbol.Code,
                TimeFrame = timeFrame.ToString(),
                Time = time,
                Comment = comment
            };

            Factory.WriteAlert(alert);

            if (Factory.IsSoundAlertEnabled)
            {
                notifications.PlaySound(Factory.SoundFilePath);
            }

            if (Factory.IsEmailAlertEnabled)
            {
                string emailSubject = string.Format("{0} {1} | Trade Alert", alert.TradeSide, alert.Symbol);

                string emailBody = string.Format(
                    "An alert triggered at {0} to {1} {2} on {3} time frame with this comment: {4}",
                    alert.Time,
                    alert.TradeSide,
                    alert.Symbol,
                    alert.TimeFrame,
                    alert.Comment);

                notifications.SendEmail(Factory.FromEmail, Factory.ToEmail, emailSubject, emailBody);
            }

            Factory.CloseWindow();

            Factory.ShowWindow();
        }

        #endregion Methods
    }
}