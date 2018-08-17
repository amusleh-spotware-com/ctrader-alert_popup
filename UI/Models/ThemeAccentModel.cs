using MahApps.Metro;
using System.Windows.Media;
using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class ThemeAccentModel
    {
        #region Properties

        public Accent Accent { get; set; }

        public Brush Color { get; set; }

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
            return other == null ? false : Accent.Name.Equals(other.Accent.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (Accent != null ? Accent.GetHashCode() : 0);

            return hash;
        }

        #endregion Methods
    }
}