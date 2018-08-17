using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace cAlgo.API.Alert.UI.Models
{
    public class TelegramOptionsModel : BindableBase
    {
        #region Fields

        private bool _isEnabled;

        private string _messageTemplate, _defaultMessageTemplate;

        private ObservableCollection<TelegramBot> _bots;

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

        public ObservableCollection<TelegramBot> Bots
        {
            get
            {
                return _bots;
            }
            set
            {
                SetProperty(ref _bots, value);
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