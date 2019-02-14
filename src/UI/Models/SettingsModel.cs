using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class SettingsModel : BindableBase
    {
        #region Fields

        private GeneralSettingsModel _general;
        private AlertSettingsModel _alerts;
        private SoundSettingsModel _sound;
        private EmailSettingsModel _email;
        private TelegramSettingsModel _telegram;

        #endregion Fields

        #region Properties

        public GeneralSettingsModel General
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

        public AlertSettingsModel Alerts
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

        public SoundSettingsModel Sound
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

        public EmailSettingsModel Email
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

        public TelegramSettingsModel Telegram
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