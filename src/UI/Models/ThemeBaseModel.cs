using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class ThemeBaseModel
    {
        #region Properties

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string SourceUri { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(ThemeBaseModel obj1, ThemeBaseModel obj2)
        {
            if (obj1 is null)
            {
                return obj2 is object;
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(ThemeBaseModel obj1, ThemeBaseModel obj2)
        {
            if (obj1 is null)
            {
                return obj2 is null;
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

            return hash;
        }

        #endregion Methods
    }
}