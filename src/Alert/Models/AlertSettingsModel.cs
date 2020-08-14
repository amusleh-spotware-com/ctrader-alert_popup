using cAlgo.API.Alert.Enums;
using cAlgo.API.Alert.Factories;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.Models
{
    public class AlertSettingsModel : BindableBase
    {
        #region Fields

        private SolidColorBrush _buyTypeColor, _sellTypeColor, _otherTypesColor, _priceColor, _symbolColor, _timeColor, _triggeredByColor, _timeFrameColor;

        private FontModel _commentFontModel;
        private int _maxAlertNumber, _maxPriceDecimalPlacesNumber;
        private TimeFormat _timeFormat;

        private TimeZoneInfo _timeZone;

        #endregion Fields

        #region Properties

        [XmlIgnore]
        public SolidColorBrush BuyTypeColor
        {
            get
            {
                return _buyTypeColor;
            }
            set
            {
                SetProperty(ref _buyTypeColor, value);
            }
        }

        public string BuyTypeColorCode
        {
            get
            {
                return BuyTypeColor.ToString();
            }
            set
            {
                BuyTypeColor = SettingsFactory.GetColorFromString(value);
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

        public int MaxPriceDecimalPlacesNumber
        {
            get
            {
                return _maxPriceDecimalPlacesNumber;
            }
            set
            {
                SetProperty(ref _maxPriceDecimalPlacesNumber, value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush OtherTypesColor
        {
            get
            {
                return _otherTypesColor;
            }
            set
            {
                SetProperty(ref _otherTypesColor, value);
            }
        }

        public string OtherTypesColorCode
        {
            get
            {
                return OtherTypesColor.ToString();
            }
            set
            {
                OtherTypesColor = SettingsFactory.GetColorFromString(value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush SellTypeColor
        {
            get
            {
                return _sellTypeColor;
            }
            set
            {
                SetProperty(ref _sellTypeColor, value);
            }
        }

        public string SellTypeColorCode
        {
            get
            {
                return SellTypeColor.ToString();
            }
            set
            {
                SellTypeColor = SettingsFactory.GetColorFromString(value);
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
                SymbolColor = SettingsFactory.GetColorFromString(value);
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
                TimeColor = SettingsFactory.GetColorFromString(value);
            }
        }

        public TimeFormat TimeFormat
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
                TimeFrameColor = SettingsFactory.GetColorFromString(value);
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
                TriggeredByColor = SettingsFactory.GetColorFromString(value);
            }
        }

        [XmlIgnore]
        public SolidColorBrush PriceColor
        {
            get
            {
                return _priceColor;
            }
            set
            {
                SetProperty(ref _priceColor, value);
            }
        }

        public string PriceColorCode
        {
            get
            {
                return PriceColor.ToString();
            }
            set
            {
                PriceColor = SettingsFactory.GetColorFromString(value);
            }
        }

        #endregion Properties
    }
}