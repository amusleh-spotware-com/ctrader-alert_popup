using System.Windows.Controls;

namespace cAlgo.API.Alert.UI.Models
{
    public class DataGridLengthSettingsModel
    {
        public double Value { get; set; }

        public DataGridLengthUnitType UnitType { get; set; }

        public double DesiredValue { get; set; }

        public double DisplayValue { get; set; }
    }
}