using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace cAlgo.API.Alert.Models
{
    public class TelegramSettingsModel : BindableBase
    {
        #region Fields

        private bool _isEnabled;

        private string _messageTemplate, _defaultMessageTemplate;

        private ObservableCollection<TelegramConversation> _conversations;

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

        public ObservableCollection<TelegramConversation> Conversations
        {
            get
            {
                return _conversations;
            }
            set
            {
                SetProperty(ref _conversations, value);
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