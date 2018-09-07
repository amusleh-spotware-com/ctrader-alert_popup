using Prism.Mvvm;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI.Models
{
    public class AlertModel : BindableBase, ICloneable
    {
        #region Fields

        private DateTimeOffset _time;
        private string _timeFrame, _symbol, _triggeredBy, _tradeSide, _comment;
        private double _price;

        #endregion Fields

        #region Properties

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

        [XmlIgnore]
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

        public string TimeString
        {
            get
            {
                return Time.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                Time = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
            }
        }

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

        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                SetProperty(ref _price, value);
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

        #region Methods

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static bool operator !=(AlertModel obj1, AlertModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return !ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(AlertModel obj1, AlertModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AlertModel))
            {
                return false;
            }

            return Equals((AlertModel)obj);
        }

        public bool Equals(AlertModel other)
        {
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;

            bool result = false;

            if (other != null &&
                Symbol.Equals(other.Symbol, stringComparison) &&
                Time.Equals(other.Time) &&
                TimeFrame.Equals(other.TimeFrame, stringComparison) &&
                TradeSide.Equals(other.TradeSide) &&
                TriggeredBy.Equals(other.TriggeredBy) &&
                Price.Equals(other.Price) &&
                Comment.Equals(other.Comment, stringComparison))
            {
                result = true;
            }

            return result;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (!string.IsNullOrEmpty(Symbol) ? Symbol.GetHashCode() : 0);
            hash += (hash * 31) + Time.GetHashCode();
            hash += (hash * 31) + (!string.IsNullOrEmpty(TimeFrame) ? TimeFrame.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(TradeSide) ? TradeSide.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(TriggeredBy) ? TriggeredBy.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(Comment) ? Comment.GetHashCode() : 0);
            hash += (hash * 31) + Price.GetHashCode();

            return hash;
        }

        #endregion Methods
    }
}