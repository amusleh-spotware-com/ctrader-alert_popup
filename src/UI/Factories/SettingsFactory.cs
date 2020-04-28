using cAlgo.API.Alert.UI.Enums;
using cAlgo.API.Alert.UI.Models;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Text;

namespace cAlgo.API.Alert.UI.Factories
{
    public static class SettingsFactory
    {
        #region Methods

        public static SettingsModel GetSettings(string path)
        {
            if (!File.Exists(path))
            {
                return GetDefaultSettings();
            }

            SettingsModel result;

            using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (TextReader reader = new StreamReader(fileStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));

                try
                {
                    result = serializer.Deserialize(reader) as SettingsModel;
                }
                catch (InvalidOperationException ex)
                {
                    fileStream.Close();

                    File.Delete(path);

                    throw ex;
                }
            }

            return result;
        }

        public static void SaveSettings(string path, SettingsModel Settings)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (TextWriter writer = new StreamWriter(fileStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SettingsModel));

                serializer.Serialize(writer, Settings);
            }
        }

        public static SolidColorBrush GetAccentColor(Accent accent)
        {
            SolidColorBrush result = typeof(Brushes).GetProperties().FirstOrDefault(
                property => property.Name.Equals(accent.Name, StringComparison.InvariantCultureIgnoreCase))?
                .GetValue(null) as SolidColorBrush
                ??
                typeof(Brushes).GetProperties().FirstOrDefault(
                property => property.Name.IndexOf(accent.Name, StringComparison.InvariantCultureIgnoreCase) >= 0)?
                .GetValue(null) as SolidColorBrush;

            if (result == null)
            {
                switch (accent.Name.ToLowerInvariant())
                {
                    case "amber":
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF0A30A"));
                        break;

                    case "emerald":
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF008A00"));
                        break;

                    case "cobalt":
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0050EF"));
                        break;

                    case "mauve":
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF76608A"));
                        break;

                    case "taupe":
                        result = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF87794E"));
                        break;

                    default:
                        result = Brushes.Transparent;
                        break;
                }
            }

            return result;
        }

        public static SolidColorBrush GetColorFromString(string colorCode)
        {
            List<SolidColorBrush> colors = GetColors();

            return colors.FirstOrDefault(color => color.ToString().Equals(colorCode, StringComparison.InvariantCultureIgnoreCase));
        }

        public static List<SolidColorBrush> GetColors()
        {
            return typeof(Brushes).GetProperties().Select(propertyInfo => (SolidColorBrush)propertyInfo.GetValue(null)).ToList();
        }

        public static string GetDefaultMessageTemplate()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("An alert triggered by {TriggeredBy} at {Time}, below is more detail:");
            stringBuilder.AppendLine("Type: {Type}");
            stringBuilder.AppendLine("Symbol: {Symbol}");
            stringBuilder.AppendLine("Price: {Price}");
            stringBuilder.AppendLine("Chart Time Frame: {TimeFrame}");
            stringBuilder.AppendLine("Comment: {Comment}");

            return stringBuilder.ToString();
        }

        public static EmailTemplateModel GetDefaultEmailTemplate()
        {
            return new EmailTemplateModel()
            {
                Subject = "{Type} {Symbol} {Price} | Trade Alert",
                Body = GetDefaultMessageTemplate()
            };
        }

        public static SettingsModel GetDefaultSettings()
        {
            GeneralSettingsModel general = new GeneralSettingsModel()
            {
                ThemeBase = new ThemeBaseModel
                {
                    Name = "BaseLight",
                    DisplayName = "Light",
                    SourceUri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml"
                },
                ThemeAccent = new ThemeAccentModel
                {
                    Name = "Cobalt",
                    ColorCode = "#FF0050EF",
                    SourceUri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Cobalt.xaml"
                },
                TopMost = false
            };

            AlertSettingsModel alerts = new AlertSettingsModel()
            {
                BuyTypeColor = Brushes.Green,
                SellTypeColor = Brushes.Red,
                OtherTypesColor = Brushes.Yellow,
                PriceColor = Brushes.SlateGray,
                SymbolColor = Brushes.DarkGoldenrod,
                TriggeredByColor = Brushes.DeepPink,
                TimeFrameColor = Brushes.DarkMagenta,
                TimeColor = Brushes.DimGray,
                MaxAlertNumber = 100,
                MaxPriceDecimalPlacesNumber = 5,
                CommentFontModel = new FontModel()
                {
                    Family = Fonts.SystemFontFamilies.FirstOrDefault(family => family.Source.Equals("Arial",
                    StringComparison.InvariantCultureIgnoreCase)),
                    WeightModel = new FontWeightModel() { Name = "Normal", Weight = FontWeights.Normal },
                    StyleModel = new FontStyleModel() { Name = "Normal", Style = FontStyles.Normal },
                    Size = 20
                },
                TimeFormat = TimeFormat.TwentyFourHour,
                TimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(tz => tz.BaseUtcOffset.Equals(DateTimeOffset.Now.Offset)),
            };

            SoundSettingsModel sound = new SoundSettingsModel();

            EmailSettingsModel email = new EmailSettingsModel()
            {
                Template = GetDefaultEmailTemplate(),
                DefaultTemplate = GetDefaultEmailTemplate(),
            };

            TelegramSettingsModel telegram = new TelegramSettingsModel()
            {
                DefaultMessageTemplate = GetDefaultTelegramMessageTemplate(),
                MessageTemplate = GetDefaultTelegramMessageTemplate(),
                Conversations = new ObservableCollection<TelegramConversation>()
            };

            SettingsModel Settings = new SettingsModel()
            {
                General = general,
                Alerts = alerts,
                Sound = sound,
                Email = email,
                Telegram = telegram
            };

            return Settings;
        }

        public static string GetDefaultTelegramMessageTemplate()
        {
            return GetDefaultMessageTemplate();
        }

        public static List<FontFamily> GetFontFamilies()
        {
            return Fonts.SystemFontFamilies.ToList();
        }

        public static FontStyle GetFontStyleFromString(string styleName)
        {
            List<FontStyleModel> styles = GetFontStyles();

            return styles.FirstOrDefault(style => style.Name.Equals(styleName, StringComparison.InvariantCultureIgnoreCase)).Style;
        }

        public static List<FontStyleModel> GetFontStyles()
        {
            return typeof(FontStyles).GetProperties().Select(propertyInfo => new FontStyleModel()
            {
                Name = propertyInfo.Name,
                Style = (FontStyle)propertyInfo.GetValue(null)
            }).ToList();
        }

        public static FontWeight GetFontWeightFromString(string weightName)
        {
            List<FontWeightModel> weights = GetFontWeights();

            return weights.FirstOrDefault(weight => weight.Name.Equals(weightName, StringComparison.InvariantCultureIgnoreCase)).Weight;
        }

        public static List<FontWeightModel> GetFontWeights()
        {
            return typeof(FontWeights).GetProperties().Select(propertyInfo => new FontWeightModel()
            {
                Name = propertyInfo.Name,
                Weight = (FontWeight)propertyInfo.GetValue(null)
            }).ToList();
        }

        public static List<ThemeAccentModel> GetThemeAccents()
        {
            return ThemeManager.Accents.Select(accent => new ThemeAccentModel
            {
                Name = accent.Name,
                Color = GetAccentColor(accent),
                SourceUri = accent.Resources.Source.ToString()
            }).ToList();
        }

        public static List<ThemeBaseModel> GetThemeBases()
        {
            return ThemeManager.AppThemes.Select(themeBase => new ThemeBaseModel
            {
                SourceUri = themeBase.Resources.Source.ToString(),
                DisplayName = themeBase.Name.Replace("Base", string.Empty),
                Name = themeBase.Name
            }).ToList();
        }

        public static List<TimeFormat> GetTimeFormats()
        {
            return Enum.GetValues(typeof(TimeFormat)).Cast<TimeFormat>().ToList();
        }

        public static List<TimeZoneInfo> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().ToList();
        }

        public static Accent GetAccent(ThemeAccentModel accentModel)
        {
            return new Accent(accentModel.Name, new Uri(accentModel.SourceUri));
        }

        public static AppTheme GetTheme(ThemeBaseModel baseModel)
        {
            return new AppTheme(baseModel.Name, new Uri(baseModel.SourceUri));
        }

        #endregion Methods
    }
}