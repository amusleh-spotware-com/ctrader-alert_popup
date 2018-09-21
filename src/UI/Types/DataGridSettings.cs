using System.Collections.Generic;
using System.Configuration;

namespace cAlgo.API.Alert.UI.Types
{
    public class DataGridSettings : ApplicationSettingsBase
    {
        public DataGridSettings()
        {
        }

        public DataGridSettings(string settingsKey) : base(settingsKey)
        {
        }

        [UserScopedSetting]
        public List<DataGridColumnSettings> ColumnsSetting
        {
            get => (List<DataGridColumnSettings>)this["ColumnsSetting"];
            set => this["ColumnsSetting"] = value;
        }
    }
}