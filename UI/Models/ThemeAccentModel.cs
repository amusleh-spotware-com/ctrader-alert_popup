using MahApps.Metro;
using System.Windows.Media;
using System;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI.Models
{
    public class ThemeAccentModel
    {
        #region Fields

        private Accent _accent;

        private string _name, _sourceUri;

        #endregion Fields

        #region Properties

        [XmlIgnore]
        public Accent Accent
        {
            get
            {
                if (_accent == null)
                {
                    _accent = new Accent(_name, new Uri(_sourceUri));
                }

                return _accent;
            }
            set
            {
                _accent = value;

                Name = value.Name;

                SourceUri = value.Resources.Source.ToString();
            }
        }

        [XmlIgnore]
        public SolidColorBrush Color { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string SourceUri
        {
            get
            {
                return _sourceUri;
            }
            set
            {
                _sourceUri = value;
            }
        }

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

            hash += (hash * 31) + (!string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0);

            return hash;
        }

        #endregion Methods
    }
}