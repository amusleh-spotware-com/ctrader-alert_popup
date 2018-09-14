using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI.Models
{
    public class ThemeAccentModel
    {
        #region Properties

        [XmlIgnore]
        public SolidColorBrush Color { get; set; }

        public string ColorCode
        {
            get
            {
                return Color.ToString();
            }
            set
            {
                Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
            }
        }

        public string Name { get; set; }

        public string SourceUri { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(ThemeAccentModel obj1, ThemeAccentModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return !ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(ThemeAccentModel obj1, ThemeAccentModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThemeAccentModel))
            {
                return false;
            }

            return Equals((ThemeAccentModel)obj);
        }

        public bool Equals(ThemeAccentModel other)
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