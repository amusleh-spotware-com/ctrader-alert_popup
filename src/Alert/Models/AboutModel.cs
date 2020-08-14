using System.Reflection;

namespace cAlgo.API.Alert.Models
{
    public class AboutMode
    {
        #region Properties

        public string Version { get; set; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string DevelopedBy { get; set; } = "AlgoDeveloper";

        public string DeveloperSite { get; set; } = "https://www.algodeveloper.com/";

        public string Github { get; set; } = "https://github.com/afhacker/ctrader-alert_popup";

        public string Ctdn { get; set; } = "https://ctdn.com/algos/indicators/show/1692";

        #endregion Properties
    }
}