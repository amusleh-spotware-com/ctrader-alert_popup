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
            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Bootstrapper bootstrapper = new Bootstrapper();

                    bootstrapper.AddAlert(new UI.Models.AlertModel { TradeSide = "Buy", Comment = "How we can trade with this", TriggeredBy = "afhacker algo", Time = DateTimeOffset.Now, Symbol = "EURUSD", TimeFrame = "1 Hour" });

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
        }
    }
}