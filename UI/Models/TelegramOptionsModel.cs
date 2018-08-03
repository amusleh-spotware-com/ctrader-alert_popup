using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class TelegramOptionsModel : BindableBase
    {
        #region Fields

        private bool _isTelegramAlertEnabled;

        #endregion Fields

        #region Properties

        public bool IsTelegramAlertEnabled
        {
            get
            {
                return _isTelegramAlertEnabled;
            }
            set
            {
                SetProperty(ref _isTelegramAlertEnabled, value);
            }
        }

        #endregion Properties
    }
}