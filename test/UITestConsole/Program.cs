using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Alert.Tester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            INotifications notifications = new Notifications();

            notifications.ShowPopup("Hour", "EURUSD", "Trend Line", "UITestConsole", 1.23452, "No comment", DateTimeOffset.Now);

            //Thread.Sleep(10000);

            //notifications.ShowPopup("Hour", "EURUSD", 1.23132, "UITestConsole", "Buy", "1", DateTimeOffset.Now);
            /*
            int counter = 0;
            while (counter < 10)
            {
                counter++;

                notifications.ShowPopup("Hour", "EURUSD", 1.23132, "UITestConsole", "Buy", counter.ToString(), DateTimeOffset.Now);

                Thread.Sleep(3000);
            }
            */
        }
    }
}