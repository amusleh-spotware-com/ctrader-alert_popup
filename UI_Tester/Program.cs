using cAlgo.API.Alert.UI;
using System;
using System.Globalization;
using System.Threading;

namespace cAlgo.API.Alert.Tester
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Bootstrapper bootstrapper = null;

            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    bootstrapper = new Bootstrapper(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\alerts.csv", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\options.xml");

                    bootstrapper.AddAlert(new UI.Models.AlertModel { TradeSide = "Buy", Comment = "Alert new comment", TriggeredBy = "afhacker algo", Time = DateTimeOffset.Now, Symbol = "EURUSD", TimeFrame = "1 Hour", Price = 1.21232342 });
                    bootstrapper.AddAlert(new UI.Models.AlertModel { TradeSide = "Buy", Comment = "Alert new comment", TriggeredBy = "afhacker algo", Time = DateTimeOffset.Now, Symbol = "EURUSD", TimeFrame = "1 Hour", Price = 1.213331342 });
                    bootstrapper.AddAlert(new UI.Models.AlertModel { TradeSide = "Buy", Comment = "Alert new comment", TriggeredBy = "afhacker algo", Time = DateTimeOffset.Now, Symbol = "EURUSD", TimeFrame = "1 Hour", Price = 1.12 });
                    bootstrapper.AddAlert(new UI.Models.AlertModel { TradeSide = "Buy", Comment = "Alert new comment", TriggeredBy = "afhacker algo", Time = DateTimeOffset.Now, Symbol = "EURUSD", TimeFrame = "1 Hour", Price = 1 });

                    bootstrapper.Run();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex);
                }
            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();

            Thread.Sleep(10000);

            bootstrapper.AddAlert(new UI.Models.AlertModel { TradeSide = "Buy", Comment = "Alert new comment", TriggeredBy = "afhacker algo", Time = DateTimeOffset.Now, Symbol = "EURUSD", TimeFrame = "1 Hour", Price = 1.21342 });

            //Telegram.Bot.TelegramBotClient client = new Telegram.Bot.TelegramBotClient("650453366:AAG--Ok1yGvv-I8jbst1zgb23gSeiIT_7_4");
            //client.GetUpdates().ToList().ForEach(update => client.SendTextMessage(new Telegram.Bot.Types.ChatId(update.Message.Chat.Id), "hi"));
        }
    }
}