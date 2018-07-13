using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using cAlgo.API.Alert.UI;
using System.Globalization;

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
                    Bootstrapper bootstrapper = new Bootstrapper("baseLight", "Blue");

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