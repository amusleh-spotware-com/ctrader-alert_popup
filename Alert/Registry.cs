using Microsoft.Win32;

namespace Alert
{
    public static class Registry
    {
        public static string KeyName { get; set; }

        public static void CreateKey(string keyName)
        {
            RegistryKey softwarekey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            RegistryKey key = softwarekey.CreateSubKey(keyName);

            softwarekey.Close();

            key.Close();

            KeyName = keyName;
        }

        public static void SetValue(string name, object value)
        {
            RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + KeyName + "\\", true);

            key.SetValue(name, value, RegistryValueKind.String);

            key.Close();
        }

        public static string GetValue(string valueName, object defaultValue)
        {
            RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + KeyName + "\\", false);

            string valueData = key.GetValue(valueName, defaultValue).ToString();

            key.Close();

            return valueData;
        }
    }
}