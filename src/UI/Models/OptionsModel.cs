using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class OptionsModel : BindableBase
    {
        #region Fields

        private GeneralOptionsModel _general;
        private AlertOptionsModel _alerts;
        private SoundOptionsModel _sound;
        private EmailOptionsModel _email;
        private TelegramOptionsModel _telegram;

        #endregion Fields

        #region Properties

        public GeneralOptionsModel General
        {
            get
            {
                return _general;
            }
            set
            {
                SetProperty(ref _general, value);
            }
        }

        public AlertOptionsModel Alerts
        {
            get
            {
                return _alerts;
            }
            set
            {
                SetProperty(ref _alerts, value);
            }
        }

        public SoundOptionsModel Sound
        {
            get
            {
                return _sound;
            }
            set
            {
                SetProperty(ref _sound, value);
            }
        }

        public EmailOptionsModel Email
        {
            get
            {
                return _email;
            }
            set
            {
                SetProperty(ref _email, value);
            }
        }

        public TelegramOptionsModel Telegram
        {
            get
            {
                return _telegram;
            }
            set
            {
                SetProperty(ref _telegram, value);
            }
        }

        #endregion Properties
    }
}