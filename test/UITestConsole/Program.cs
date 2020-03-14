using cAlgo.API.Internals;
using System;
using System.Threading;

namespace cAlgo.API.Alert.Tester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            INotifications notifications = new Notifications();

            //notifications.ShowPopup();

            //notifications.ShowPopup("Hour", "EURUSD", "Trend Line", "UITestConsole", 1.23452, "No comment 2", DateTimeOffset.Now);
            /*
            int counter = 0;

            string[] arrows = { "1", "2", "3" };

            while (counter < 100)
            {
                counter++;

                string type = string.Format("Trend Line {0}", arrows[new Random().Next(0, 3)]);

                notifications.ShowPopup("Hour", "EURUSD", type, "UITestConsole", 1.23452, counter.ToString(), DateTimeOffset.Now);

                Thread.Sleep(2000);
            }*/
        }
    }
}