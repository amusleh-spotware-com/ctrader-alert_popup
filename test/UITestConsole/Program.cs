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

            notifications.ShowPopup("Hour", "EURUSD", 1.23132, "UITestConsole", "Buy", "2", DateTimeOffset.Now);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            notifications.ShowPopup("Hour", "EURUSD", 1.33333, "UITestConsole", "Sell", "10", DateTimeOffset.Now);

            //Telegram.Bot.TelegramBotClient client = new Telegram.Bot.TelegramBotClient("650453366:AAG--Ok1yGvv-I8jbst1zgb23gSeiIT_7_4");
            //client.GetUpdates().ToList().ForEach(update => client.SendTextMessage(new Telegram.Bot.Types.ChatId(update.Message.Chat.Id), "hi"));
        }
    }
}