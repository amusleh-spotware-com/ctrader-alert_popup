using Prism.Mvvm;
using System;
using System.Globalization;
using System.Xml.Serialization;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace cAlgo.API.Alert.UI.Models
{
    public class AlertModel : BindableBase, ICloneable
    {
        #region Fields

        private DateTimeOffset _time;
        private string _timeFrame, _symbol, _triggeredBy, _type, _comment;
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

        public bool Equals(AlertModel other)
        {
            if (other == null)
            {
                return false;
            }

            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties().Where(property => property.CanRead);

            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;

            foreach (PropertyInfo property in properties)
            {
                object currentValue = property.GetValue(this);
                object otherValue = property.GetValue(other);

                if ((currentValue != null && otherValue == null) || (currentValue == null && otherValue != null))
                {
                    return false;
                }
                else if (currentValue != null && otherValue != null)
                {
                    if (property.PropertyType == typeof(string) && !currentValue.ToString().Equals(otherValue.ToString(), stringComparison))
                    {
                        return false;
                    }
                    else if (!currentValue.Equals(otherValue))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (!string.IsNullOrEmpty(Symbol) ? Symbol.GetHashCode() : 0);
            hash += (hash * 31) + Time.GetHashCode();
            hash += (hash * 31) + (!string.IsNullOrEmpty(TimeFrame) ? TimeFrame.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(Type) ? Type.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(TriggeredBy) ? TriggeredBy.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(Comment) ? Comment.GetHashCode() : 0);
            hash += (hash * 31) + Price.GetHashCode();

            return hash;
        }

        #endregion Methods
    }
}