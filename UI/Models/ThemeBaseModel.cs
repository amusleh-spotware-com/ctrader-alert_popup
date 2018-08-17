using MahApps.Metro;
using System;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.UI.Models
{
    public class ThemeBaseModel
    {
        #region Fields

        private AppTheme _base;

        private string _name, _sourceUri;

        #endregion Fields

        #region Properties

        [XmlIgnore]
        public AppTheme Base
        {
            get
            {
                if (_base == null)
                {
                    _base = new AppTheme(_name, new Uri(_sourceUri));
                }

                return _base;
            }
            set
            {
                _base = value;

                Name = value.Name;

                SourceUri = value.Resources.Source.ToString();
            }
        }

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

            return hash;
        }

        #endregion Methods
    }
}