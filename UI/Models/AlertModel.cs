using Prism.Mvvm;
using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class AlertModel : BindableBase
    {
        #region Fields

        private string _timeFrame, _symbol, _triggeredBy, _tradeSide, _comment;

        private DateTimeOffset _time;

        #endregion Fields

        #region Properties

        public string TimeFrame
        {
            get
            {
                return _timeFrame;
            }
            set
            {
                SetProperty(ref _timeFrame, value);
            }
        }

        public string Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                SetProperty(ref _symbol, value);
            }
        }

        public string TriggeredBy
        {
            get
            {
                return _triggeredBy;
            }
            set
            {
                SetProperty(ref _triggeredBy, value);
            }
        }

        public DateTimeOffset Time
        {
            get
            {
                return _time;
            }
            set
            {
                SetProperty(ref _time, value);
            }
        }

        public string TradeSide
        {
            get
            {
                return _tradeSide;
            }
            set
            {
                SetProperty(ref _tradeSide, value);
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                SetProperty(ref _comment, value);
            }
        }

        #endregion Properties
    }
}