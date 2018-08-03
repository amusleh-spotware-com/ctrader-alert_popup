using System;
using System.Windows;

namespace cAlgo.API.Alert.UI.Models
{
    public class FontStyleModel
    {
        #region Properties

        public string Name { get; set; }

        public FontStyle Style { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(FontStyleModel obj1, FontStyleModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return !ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(FontStyleModel obj1, FontStyleModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
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
            hash += (hash * 31) + (Style != null ? Style.GetHashCode() : 0);

            return hash;
        }

        #endregion Methods
    }
}