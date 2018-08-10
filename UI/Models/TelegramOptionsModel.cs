using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class TelegramOptionsModel : BindableBase
    {
        #region Fields

        private bool _isEnabled;

        private string _botToken, _messageTemplate, _defaultMessageTemplate;

        #endregion Fields

        #region Properties

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                SetProperty(ref _isEnabled, value);
            }
        }

        public string BotToken
        {
            get
            {
                return _botToken;
            }
            set
            {
                SetProperty(ref _botToken, value);
            }
        }

        public string MessageTemplate
        {
            get
            {
                return _messageTemplate;
            }
            set
            {
                SetProperty(ref _messageTemplate, value);
            }
        }

        public string DefaultMessageTemplate
        {
            get
            {
                return _defaultMessageTemplate;
            }
            set
            {
                SetProperty(ref _defaultMessageTemplate, value);
            }
        }

        #endregion Properties
    }
}