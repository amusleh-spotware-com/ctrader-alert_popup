using cAlgo.API.Alert.UI.Types.Enums;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public static class OptionsBaseViewModel
    {
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

        public static Models.EmailTemplateModel GetDefaultEmailTemplate()
        {
            return new Models.EmailTemplateModel()
            {
                Subject = "{TradeSide} {Symbol} {Price} | Trade Alert",
                Body = "An alert triggered by {TriggeredBy} at {Time} to {TradeSide} {Symbol} at price {Price} on {TimeFrame} time frame, comment: {Comment}"
            };
        }

        public static Models.OptionsModel GetDefaultOptions()
        {
            Models.GeneralOptionsModel general = new Models.GeneralOptionsModel()
            {
                ThemeBase = new Models.ThemeBaseModel
                {
                    Name = "BaseLight",
                    DisplayName = "Light",
                    SourceUri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml"
                },
                ThemeAccent = new Models.ThemeAccentModel
                {
                    Name = "Cobalt",
                    ColorCode = "#FF0050EF",
                    SourceUri = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Cobalt.xaml"
                },
                TopMost = true
            };

            Models.AlertOptionsModel alerts = new Models.AlertOptionsModel()
            {
                BuyTypeColor = Brushes.Green,
                SellTypeColor = Brushes.Red,
                OtherTypesColor = Brushes.Yellow,
                PriceColor = Brushes.SlateGray,
                SymbolColor = Brushes.DarkGoldenrod,
                TriggeredByColor = Brushes.DeepPink,
                TimeFrameColor = Brushes.DarkMagenta,
                TimeColor = Brushes.DimGray,
                MaxAlertNumber = 200,
                MaxPriceDecimalPlacesNumber = 5,
                CommentFontModel = new Models.FontModel()
                {
                    Family = Fonts.SystemFontFamilies.FirstOrDefault(family => family.Source.Equals("Arial",
                    StringComparison.InvariantCultureIgnoreCase)),
                    WeightModel = new Models.FontWeightModel() { Name = "Normal", Weight = FontWeights.Normal },
                    StyleModel = new Models.FontStyleModel() { Name = "Normal", Style = FontStyles.Normal },
                    Size = 20
                },
                TimeFormat = TimeFormat.TwentyFourHour,
                TimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(tz => tz.BaseUtcOffset.Equals(DateTimeOffset.Now.Offset)),
            };

            Models.SoundOptionsModel sound = new Models.SoundOptionsModel();

            Models.EmailOptionsModel email = new Models.EmailOptionsModel()
            {
                Template = GetDefaultEmailTemplate(),
                DefaultTemplate = GetDefaultEmailTemplate(),
            };

            Models.TelegramOptionsModel telegram = new Models.TelegramOptionsModel()
            {
                DefaultMessageTemplate = GetDefaultTelegramMessageTemplate(),
                MessageTemplate = GetDefaultTelegramMessageTemplate(),
                Conversations = new ObservableCollection<Models.TelegramConversation>()
            };

            Models.OptionsModel options = new Models.OptionsModel()
            {
                General = general,
                Alerts = alerts,
                Sound = sound,
                Email = email,
                Telegram = telegram
            };

            return options;
        }

        public static string GetDefaultTelegramMessageTemplate()
        {
            return "An alert triggered by {TriggeredBy} at {Time} to {TradeSide} {Symbol} at price {Price} on {TimeFrame} time frame, comment: {Comment}";
        }

        public static List<FontFamily> GetFontFamilies()
        {
            return Fonts.SystemFontFamilies.ToList();
        }

        public static FontStyle GetFontStyleFromString(string styleName)
        {
            List<Models.FontStyleModel> styles = GetFontStyles();

            return styles.FirstOrDefault(style => style.Name.Equals(styleName, StringComparison.InvariantCultureIgnoreCase)).Style;
        }

        public static List<Models.FontStyleModel> GetFontStyles()
        {
            return typeof(FontStyles).GetProperties().Select(propertyInfo => new Models.FontStyleModel()
            {
                Name = propertyInfo.Name,
                Style = (FontStyle)propertyInfo.GetValue(null)
            }).ToList();
        }

        public static FontWeight GetFontWeightFromString(string weightName)
        {
            List<Models.FontWeightModel> weights = GetFontWeights();

            return weights.FirstOrDefault(weight => weight.Name.Equals(weightName, StringComparison.InvariantCultureIgnoreCase)).Weight;
        }

        public static List<Models.FontWeightModel> GetFontWeights()
        {
            return typeof(FontWeights).GetProperties().Select(propertyInfo => new Models.FontWeightModel()
            {
                Name = propertyInfo.Name,
                Weight = (FontWeight)propertyInfo.GetValue(null)
            }).ToList();
        }

        public static List<Models.ThemeAccentModel> GetThemeAccents()
        {
            return ThemeManager.Accents.Select(accent => new Models.ThemeAccentModel
            {
                Name = accent.Name,
                Color = GetAccentColor(accent),
                SourceUri = accent.Resources.Source.ToString()
            }).ToList();
        }

        public static List<Models.ThemeBaseModel> GetThemeBases()
        {
            return ThemeManager.AppThemes.Select(themeBase => new Models.ThemeBaseModel
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

        public static Accent GetAccent(Models.ThemeAccentModel accentModel)
        {
            return new Accent(accentModel.Name, new Uri(accentModel.SourceUri));
        }

        public static AppTheme GetTheme(Models.ThemeBaseModel baseModel)
        {
            return new AppTheme(baseModel.Name, new Uri(baseModel.SourceUri));
        }
    }
}