using System;
using System.ComponentModel;
using System.Windows;

namespace cAlgo.API.Alert.UI.Models
{
    public class DataGridColumnSettingsModel
    {
        #region Properties

        public string Header { get; set; }

        public int DisplayIndex { get; set; }

        public DataGridLengthSettingsModel Width { get; set; }

        public ListSortDirection? SortDirection { get; set; }

        public Visibility Visibility { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(DataGridColumnSettingsModel obj1, DataGridColumnSettingsModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return !ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(DataGridColumnSettingsModel obj1, DataGridColumnSettingsModel obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataGridColumnSettingsModel))
            {
                return false;
            }

            return Equals((DataGridColumnSettingsModel)obj);
        }

        public bool Equals(DataGridColumnSettingsModel other)
        {
            if (other == null)
            {
                return false;
            }

            return Header.Equals(other.Header, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (Header == null ? 0 : Header.GetHashCode());

            return hash;
        }

        #endregion Methods
    }
}