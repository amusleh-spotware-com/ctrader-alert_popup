using Prism.Mvvm;
using System.Windows.Media;
using System;
using System.Xml.Serialization;
using System.Linq;
using System.Globalization;

namespace cAlgo.API.Alert.UI.Models
{
    public class AlertOptionsModel : BindableBase
    {
        #region Fields

        private SolidColorBrush _buySideColor, _sellSideColor, _neutralSideColor, _symbolColor, _timeColor, _triggeredByColor, _timeFrameColor;

        private FontModel _commentFontModel;
        private int _maxAlertNumber;
        private Types.TimeFormat _timeFormat;

        private TimeZoneInfo _timeZone;

        #endregion Fields

        #region Properties

        [XmlIgnore]
        public SolidColorBrush BuySideColor
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

        public string BuySideColorCode
        {
            get
            {
                return BuySideColor.ToString();
            }
            set
            {
                BuySideColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        public FontModel CommentFontModel
        {
            get
            {
                return _commentFontModel;
            }
            set
            {
                SetProperty(ref _commentFontModel, value);
            }
        }

        public int MaxAlertNumber
        {
            get
            {
                return _maxAlertNumber;
            }
            set
            {
                SetProperty(ref _maxAlertNumber, value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush NeutralSideColor
        {
            get
            {
                return _neutralSideColor;
            }
            set
            {
                SetProperty(ref _neutralSideColor, value);
            }
        }

        public string NeutralSideColorCode
        {
            get
            {
                return NeutralSideColor.ToString();
            }
            set
            {
                NeutralSideColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush SellSideColor
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

        public string SellSideColorCode
        {
            get
            {
                return SellSideColor.ToString();
            }
            set
            {
                SellSideColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush SymbolColor
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

        public string SymbolColorCode
        {
            get
            {
                return SymbolColor.ToString();
            }
            set
            {
                SymbolColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush TimeColor
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

        public string TimeColorCode
        {
            get
            {
                return TimeColor.ToString();
            }
            set
            {
                TimeColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        public Types.TimeFormat TimeFormat
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

        [XmlIgnore]
        public SolidColorBrush TimeFrameColor
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

        public string TimeFrameColorCode
        {
            get
            {
                return TimeFrameColor.ToString();
            }
            set
            {
                TimeFrameColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        [XmlIgnore]
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

        public string TimeZoneName
        {
            get
            {
                return TimeZone.DisplayName;
            }
            set
            {
                TimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(tz => tz.DisplayName.Equals(value, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [XmlIgnore]
        public SolidColorBrush TriggeredByColor
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

        public string TriggeredByColorCode
        {
            get
            {
                return TriggeredByColor.ToString();
            }
            set
            {
                TriggeredByColor = ViewModels.OptionsBaseViewModel.GetColorFromString(value);
            }
        }

        #endregion Properties
    }
}