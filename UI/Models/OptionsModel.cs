using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using MahApps.Metro;
using System.Windows.Media;

namespace cAlgo.API.Alert.UI.Models
{
    public class OptionsModel : BindableBase
    {
        #region Fields

        private Enums.TimeFormat _timeFormat;

        private TimeZoneInfo _timeZone;

        private bool _isSoundAlertEnabled, _isEmailAlertEnabled, _isTelegramAlertEnabled;

        private string _senderEmail, _recipientEmail, _soundFilePath;

        private AppTheme _themeBase;

        private Accent _themeAccent;

        private Brush _buySideColor, _sellSideColor, _symbolColor, _timeColor, _triggeredByColor, _timeFrameColor;

        #endregion Fields

        #region Properties

        public Enums.TimeFormat TimeFormat
        {
            get
            {
                return _timeFormat;
            }
            set
            {
                SetProperty(ref _timeFormat, value);
            }
        }

        public TimeZoneInfo TimeZone
        {
            get
            {
                return _timeZone;
            }
            set
            {
                SetProperty(ref _timeZone, value);
            }
        }

        public bool IsSoundAlertEnabled
        {
            get
            {
                return _isSoundAlertEnabled;
            }
            set
            {
                SetProperty(ref _isSoundAlertEnabled, value);
            }
        }

        public bool IsEmailAlertEnabled
        {
            get
            {
                return _isEmailAlertEnabled;
            }
            set
            {
                SetProperty(ref _isEmailAlertEnabled, value);
            }
        }

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

        public string SenderEmail
        {
            get
            {
                return _senderEmail;
            }
            set
            {
                SetProperty(ref _senderEmail, value);
            }
        }

        public string RecipientEmail
        {
            get
            {
                return _recipientEmail;
            }
            set
            {
                SetProperty(ref _recipientEmail, value);
            }
        }

        public string SoundFilePath
        {
            get
            {
                return _soundFilePath;
            }
            set
            {
                SetProperty(ref _soundFilePath, value);
            }
        }

        public AppTheme ThemeBase
        {
            get
            {
                return _themeBase;
            }
            set
            {
                SetProperty(ref _themeBase, value);
            }
        }

        public Accent ThemeAccent
        {
            get
            {
                return _themeAccent;
            }
            set
            {
                SetProperty(ref _themeAccent, value);
            }
        }

        public Brush BuySideColor
        {
            get
            {
                return _buySideColor;
            }
            set
            {
                SetProperty(ref _buySideColor, value);
            }
        }

        public Brush SellSideColor
        {
            get
            {
                return _sellSideColor;
            }
            set
            {
                SetProperty(ref _sellSideColor, value);
            }
        }

        public Brush SymbolColor
        {
            get
            {
                return _symbolColor;
            }
            set
            {
                SetProperty(ref _symbolColor, value);
            }
        }

        public Brush TimeColor
        {
            get
            {
                return _timeColor;
            }
            set
            {
                SetProperty(ref _timeColor, value);
            }
        }

        public Brush TriggeredByColor
        {
            get
            {
                return _triggeredByColor;
            }
            set
            {
                SetProperty(ref _triggeredByColor, value);
            }
        }

        public Brush TimeFrameColor
        {
            get
            {
                return _timeFrameColor;
            }
            set
            {
                SetProperty(ref _timeFrameColor, value);
            }
        }

        #endregion Properties
    }
}