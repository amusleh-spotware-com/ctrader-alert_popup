using cAlgo.API.Alert.UI.Models;
using cAlgo.API.Internals;
using System;
using System.Threading;

namespace cAlgo.API.Alert.Tester
{
    public class Program
    {
        private static INotifications _notifications = new Notifications();

        private static void Main(string[] args)
        {
            _notifications.ShowPopup();

            //_notifications.ShowPopup("Hour", "EURUSD", "Trend Line", "UITestConsole", 1.23452, "No comment 2", DateTimeOffset.Now);
            /*
            int counter = 0;

            string[] arrows = { "1", "2", "3" };

            while (counter < 100)
            {
                counter++;

                string type = string.Format("Trend Line {0}", arrows[new Random().Next(0, 3)]);

                _notifications.ShowPopup("Hour", "EURUSD", type, "UITestConsole", 1.23452, counter.ToString(), DateTimeOffset.Now);

                Thread.Sleep(2000);
            }*/

            //TestMultiThread();
        }

        private static void TestMultiThread()
        {
            for (int i = 0; i < 10; i++)
            {
                var alert = new AlertModel
                {
                    Comment = i.ToString(),
                    TimeFrame = "Hour",
                    Price = 1.12234,
                    Symbol = "EURUSD",
                    Time = DateTimeOffset.Now.AddHours(i),
                    TriggeredBy = "UITestConsole",
                    Type = "Buy"
                };

                var thread = new Thread(() =>
                {
                    _notifications.ShowPopup(alert);
                });

                thread.Start();
            }
        }
    }
}