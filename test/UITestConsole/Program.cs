using cAlgo.API.Alert.UI.Models;
using System;
using System.Threading;

namespace cAlgo.API.Alert.Tester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var notifications = new Notifications();

            notifications.ShowPopup();

            //notifications.ShowPopup("Hour", "EURUSD", "Trend Line", "UITestConsole", 1.23452, "No comment 2", DateTimeOffset.Now);

            //TriggerAlerts(50, notifications);

            //TestMultiThread();
        }

        private static void TriggerAlerts(int number, Notifications notifications)
        {
            int counter = 0;

            string[] arrows = { "1", "2", "3" };

            while (counter < number)
            {
                counter++;

                var time = DateTimeOffset.Now.AddDays(number).AddMinutes(counter);

                var alert = new AlertModel
                {
                    TimeFrame = "Hour",
                    Type = string.Format("Trend Line {0}", arrows[new Random().Next(0, 3)]),
                    Symbol = "EURUSD",
                    Time = time,
                    Price = 1.23456,
                    Comment = counter.ToString(),
                    TriggeredBy = "UITestConsole"
                };

                notifications.TriggerAlert(alert);
            }
        }

        private static void TestMultiThread()
        {
            for (int i = 0; i < 20; i++)
            {
                int number = i;

                var thread = new Thread(() =>
                {
                    var notifications = new Notifications();

                    TriggerAlerts(number, notifications);
                });

                thread.Start();
            }
        }
    }
}