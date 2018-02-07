using MahApps.Metro;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System;
using System.Linq;

namespace Alert.Models
{
    public class SettingsModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public bool IsSoundAlertEnabled
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsSoundAlertEnabled", false));
            }
            set
            {
                Registry.SetValue("IsSoundAlertEnabled", value);

                OnPropertyChanged("IsSoundAlertEnabled");
            }
        }

        public string SoundFilePath
        {
            get
            {
                return Registry.GetValue("SoundFilePath", string.Empty);
            }
            set
            {
                Registry.SetValue("SoundFilePath", value);

                OnPropertyChanged("SoundFilePath");
            }
        }

        public bool IsEmailAlertEnabled
        {
            get
            {
                return bool.Parse(Registry.GetValue("IsEmailAlertEnabled", false));
            }
            set
            {
                Registry.SetValue("IsEmailAlertEnabled", value);

                OnPropertyChanged("IsEmailAlertEnabled");
            }
        }

        public string FromEmail
        {
            get
            {
                return Registry.GetValue("FromEmail", "");
            }
            set
            {
                Registry.SetValue("FromEmail", value);

                OnPropertyChanged("FromEmail");
            }
        }

        public string ToEmail
        {
            get
            {
                return Registry.GetValue("ToEmail", string.Empty);
            }
            set
            {
                Registry.SetValue("ToEmail", value);

                OnPropertyChanged("ToEmail");
            }
        }

        public AppTheme CurrentTheme
        {
            get
            {
                return ThemeManager.GetAppTheme(Registry.GetValue("CurrentTheme", "BaseDark"));
            }
            set
            {
                Registry.SetValue("CurrentTheme", value.Name);

                OnPropertyChanged("CurrentTheme");
            }
        }

        public Accent CurrentAccent
        {
            get
            {
                return ThemeManager.GetAccent(Registry.GetValue("CurrentAccent", "Blue"));
            }
            set
            {
                Registry.SetValue("CurrentAccent", value.Name);

                OnPropertyChanged("CurrentAccent");
            }
        }

        public ObservableCollection<AppTheme> Themes
        {
            get
            {
                return new ObservableCollection<AppTheme>(ThemeManager.AppThemes);
            }
        }

        public ObservableCollection<Accent> Accents
        {
            get
            {
                return new ObservableCollection<Accent>(ThemeManager.Accents);
            }
        }

        public int MaximumAlertsNumberToShow
        {
            get
            {
                return int.Parse(Registry.GetValue("MaximumAlertsNumberToShow", 50), CultureInfo.InvariantCulture);
            }
            set
            {
                Registry.SetValue("MaximumAlertsNumberToShow", value);

                OnPropertyChanged("MaximumAlertsNumberToShow");
            }
        }

        public ReadOnlyCollection<TimeZoneInfo> TimeZones
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones();
            }
        }

        public TimeZoneInfo CurrentTimeZone
        {
            get
            {
                string timeZone = Registry.GetValue("CurrentTimeZone", string.Empty);

                return !string.IsNullOrEmpty(timeZone) ?
                    TimeZones.FirstOrDefault(tz => tz.DisplayName.Equals(timeZone, StringComparison.InvariantCultureIgnoreCase)) :
                    TimeZones.FirstOrDefault(
                        tz => tz.DisplayName.Equals("(UTC) Coordinated Universal Time", StringComparison.InvariantCultureIgnoreCase));
            }
            set
            {
                Registry.SetValue("CurrentTimeZone", value);

                OnPropertyChanged("CurrentTimeZone");
            }
        }

        public ObservableCollection<string> TimeFormats
        {
            get
            {
                return new ObservableCollection<string>() { "12 Hour", "24 Hour" };
            }
        }

        public string CurrentTimeFormat
        {
            get
            {
                return Registry.GetValue("CurrentTimeFormat", "12 Hour");
            }
            set
            {
                Registry.SetValue("CurrentTimeFormat", value);

                OnPropertyChanged("CurrentTimeFormat");
            }
        }

        #endregion Properties

        #region Methods

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods
    }
}