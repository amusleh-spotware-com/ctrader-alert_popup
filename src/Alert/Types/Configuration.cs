using System;

namespace cAlgo.API.Alert.Types
{
    public static class Configuration
    {
        #region Properties

        public static string AlertFilePath { get; set; }
        public static string OptionsFilePath { get; set; }
        public static Action<string> Tracer { get; set; }
        public static bool? SinglePopupWindow { get; set; }
        public static string Title { get; set; }

        #endregion Properties
    }
}