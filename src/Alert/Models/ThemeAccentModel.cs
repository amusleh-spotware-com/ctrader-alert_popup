using System;
using System.Xml.Serialization;
using media = System.Windows.Media;

namespace cAlgo.API.Alert.Models
{
    public class ThemeAccentModel
    {
        #region Properties

        [XmlIgnore]
        public media.SolidColorBrush Color { get; set; }

        public string ColorCode
        {
            get
            {
                return Color.ToString();
            }
            set
            {
                Color = new media.SolidColorBrush((media.Color)media.ColorConverter.ConvertFromString(value));
            }
        }

        public string Name { get; set; }

        public string SourceUri { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(ThemeAccentModel obj1, ThemeAccentModel obj2)
        {
            if (obj1 is null)
            {
                return obj2 is object;
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(ThemeAccentModel obj1, ThemeAccentModel obj2)
        {
            if (obj1 is null)
            {
                return obj2 is null;
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
            return other != null && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
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