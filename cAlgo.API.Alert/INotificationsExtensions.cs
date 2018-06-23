using cAlgo.API.Internals;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace cAlgo.API.Alert
{
    public static class INotificationsExtensions
    {
        #region Fields

        private static int? lastTriggeredBar = null;

        #endregion Fields

        #region Methods

        public static void ShowPopup(
            this INotifications notifications,
            Algo algo,
            TradeType tradeType,
            Symbol symbol,
            TimeFrame timeFrame,
            DateTimeOffset time,
            string comment,
            TriggerType type = TriggerType.PerBar)
        {
            Factory.Algo = algo;

            if (type == TriggerType.PerBar)
            {
                int index = algo.MarketSeries.Close.Count - 1;

                if (lastTriggeredBar.HasValue && lastTriggeredBar == index)
                {
                    return;
                }
                else
                {
                    lastTriggeredBar = index;
                }
            }

            if (algo.GetType().BaseType == typeof(Indicator) && !(algo as Indicator).IsLastBar)
            {
                return;
            }

            Registry.CreateKey("cTrader Alert");

            Alert alert = new Alert() { TradeSide = tradeType.ToString(), Symbol = symbol.Code, TimeFrame = timeFrame.ToString(), Time = time, Comment = comment };

            Factory.WriteAlert(alert);

            if (Factory.IsSoundAlertEnabled)
            {
                notifications.PlaySound(Factory.SoundFilePath);
            }

            if (Factory.IsEmailAlertEnabled)
            {
                string emailSubject = string.Format("{0} {1} | Trade Alert", alert.TradeSide, alert.Symbol);

                string emailBody = string.Format("An alert triggered at {0} to {1} {2} on {3} time frame with this comment: {4}", alert.Time, alert.TradeSide, alert.Symbol, alert.TimeFrame, alert.Comment);

                notifications.SendEmail(Factory.FromEmail, Factory.ToEmail, emailSubject, emailBody);
            }

            Factory.CloseWindow();

            Factory.ShowWindow();
        }

        #endregion Methods
    }
}