using System;
using System.Windows;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI.Models
{
    public class FontStyleModel
    {
        #region Fields

        private string _name;

        #endregion Fields

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                Style = (FontStyle)typeof(FontStyles).GetProperty(_name).GetValue(null);
            }
        }

        [XmlIgnore]
        public FontStyle Style { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(FontStyleModel obj1, FontStyleModel obj2)
        {
            if (obj1 is null)
            {
                return obj2 is object;
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(FontStyleModel obj1, FontStyleModel obj2)
        {
            if (obj1 is null)
            {
                return obj2 is null;
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FontStyleModel))
            {
                return false;
            }

            return Equals((FontStyleModel)obj);
        }

        public bool Equals(FontStyleModel other)
        {
            return other == null ? false : Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (!string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0);

            return hash;
        }

        #endregion Methods
    }
}