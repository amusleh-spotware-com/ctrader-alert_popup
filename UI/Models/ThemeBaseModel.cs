using MahApps.Metro;
using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class ThemeBaseModel
    {
        #region Properties

        public AppTheme Base { get; set; }

        public string Name { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(ThemeBaseModel obj1, ThemeBaseModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return !ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(ThemeBaseModel obj1, ThemeBaseModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThemeBaseModel))
            {
                return false;
            }

            return Equals((ThemeBaseModel)obj);
        }

        public bool Equals(ThemeBaseModel other)
        {
            return other == null ? false : Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (!string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0);
            hash += (hash * 31) + (Base != null ? Base.GetHashCode() : 0);

            return hash;
        }

        #endregion Methods
    }
}