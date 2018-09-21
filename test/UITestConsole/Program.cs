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

            int counter = 0;
            while (counter < 20)
            {
                counter++;

                notifications.ShowPopup("Hour", "EURUSD", 1.23132, "UITestConsole", "Buy", counter.ToString(), DateTimeOffset.Now);

                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }
        }
    }
}