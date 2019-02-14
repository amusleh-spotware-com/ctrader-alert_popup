using LiteDB;
using Prism.Mvvm;
using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class AlertModel : BindableBase, ICloneable
    {
        #region Fields

        private DateTimeOffset _time;

        private string _id, _timeFrame, _symbol, _triggeredBy, _type, _comment;

        private double _price;

        #endregion Fields

        #region Properties

        [BsonId]
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = $"{Time.Ticks}_{Symbol}_{Type}";
                }

                return _id;
            }
            set => _id = value;
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

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                SetProperty(ref _type, value);
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

        public bool Equals(AlertModel other) => other != null && other.Id.Equals(Id, StringComparison.InvariantCultureIgnoreCase);

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (!string.IsNullOrEmpty(Id) ? Id.GetHashCode() : 0);

            return hash;
        }

        #endregion Methods
    }
}