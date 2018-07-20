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