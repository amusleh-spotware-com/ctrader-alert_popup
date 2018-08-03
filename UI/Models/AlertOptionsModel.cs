using Prism.Mvvm;
using System.Windows.Media;
using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class AlertOptionsModel : BindableBase
    {
        #region Fields

        private Brush _buySideColor, _sellSideColor, _symbolColor, _timeColor, _triggeredByColor, _timeFrameColor;

        private int _maxAlertNumber;

        private FontModel _commentFontModel;

        private Enums.TimeFormat _timeFormat;

        private TimeZoneInfo _timeZone;

        #endregion Fields

        #region Properties

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

        #endregion Properties
    }
}