using System;

namespace cAlgo.API.Alert
{
    public static class Configuration
    {
        #region Properties

        public static string AlertFilePath { get; set; }
        public static string OptionsFilePath { get; set; }
        public static Action<string> Tracer { get; set; }

        #endregion Properties
    }
}