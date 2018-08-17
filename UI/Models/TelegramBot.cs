using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class TelegramBot : BindableBase
    {
        #region Fields

        private string _name, _token;

        #endregion Fields

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public string Token
        {
            get
            {
                return _token;
            }
            set
            {
                SetProperty(ref _token, value);
            }
        }

        #endregion Properties
    }
}