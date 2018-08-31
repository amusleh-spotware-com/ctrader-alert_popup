using cAlgo.API.Alert.UI.Models;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using Telegram.Bot;

namespace cAlgo.API.Alert
{
    public static class Controller
    {
        #region Methods

        public static void LogException(Exception ex)
        {
            Configuration.Tracer?.Invoke(string.Format("Exception Message: {0}", ex.Message));
            Configuration.Tracer?.Invoke(string.Format("Exception Type: {0}", ex.GetType()));
            Configuration.Tracer?.Invoke(string.Format("Exception TargetSite: {0}", ex.TargetSite));
            Configuration.Tracer?.Invoke(string.Format("Exception Source: {0}", ex.Source));
            Configuration.Tracer?.Invoke(string.Format("Exception StackTrace: {0}", ex.StackTrace));
            Configuration.Tracer?.Invoke(string.Format("Exception InnerException: {0}", ex.InnerException));
        }

        public static void PlaySound(string path)
        {
            try
            {
                SoundPlayer soundPlayer = new SoundPlayer(path);

                soundPlayer.PlaySync();

                soundPlayer.Dispose();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public static string PutObjectInTemplate(object obj, string template)
        {
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

            List<string> keywords = template.Split().Where(word => word.StartsWith("{", comparison) && word.EndsWith("}", comparison)).ToList();

            Dictionary<string, object> properties = obj.GetType().GetProperties().ToDictionary(propertyInfo => propertyInfo.Name,
                propertyInfo => propertyInfo.GetValue(obj));

            keywords.ForEach(keyword => template = template.Replace(keyword, properties[RemoveKeywordBrackets(keyword)].ToString()));

            return template;
        }

        public static string RemoveKeywordBrackets(string word)
        {
            return new string(word.Skip(1).Take(word.Length - 2).ToArray());
        }

        public static void SendEmail(INotifications notifications, EmailOptionsModel options, AlertModel alert)
        {
            try
            {
                string subject = PutObjectInTemplate(alert, options.Template.Subject);
                string body = PutObjectInTemplate(alert, options.Template.Body);

                notifications.SendEmail(options.Sender, options.Recipient, subject, body);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public static void SendTelegramMessage(TelegramOptionsModel options, AlertModel alert)
        {
            try
            {
                string message = PutObjectInTemplate(alert, options.MessageTemplate);

                foreach (TelegramConversation conversation in options.Conversations)
                {
                    TelegramBotClient telegramBotClient = new TelegramBotClient(conversation.BotToken);

                    telegramBotClient.SendTextMessage(conversation.Id, message);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public static void SetupConfigurationPaths()
        {
            if (string.IsNullOrEmpty(Configuration.AlertFilePath) || string.IsNullOrEmpty(Configuration.OptionsFilePath))
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string calgoDirPath = Path.Combine(documentsPath, "cAlgo");

                if (!Directory.Exists(calgoDirPath))
                {
                    Directory.CreateDirectory(calgoDirPath);
                }

                Configuration.AlertFilePath = Configuration.AlertFilePath ?? Path.Combine(calgoDirPath, "Alerts.csv");
                Configuration.OptionsFilePath = Configuration.OptionsFilePath ?? Path.Combine(calgoDirPath, "PopupOptions.xml");
            }
        }

        public static void TriggerAlerts(INotifications notifications, OptionsModel options, AlertModel alert)
        {
            if (options.Sound.IsEnabled)
            {
                PlaySound(options.Sound.FilePath);
            }

            AlertModel alertCopy = alert.Clone() as AlertModel;

            alertCopy.Price = Math.Round(alertCopy.Price, options.Alerts.MaxPriceDecimalPlacesNumber);

            if (!alertCopy.Time.Offset.Equals(options.Alerts.TimeZone.BaseUtcOffset))
            {
                alertCopy.Time = alert.Time.ToOffset(options.Alerts.TimeZone.BaseUtcOffset);
            }

            if (options.Email.IsEnabled)
            {
                SendEmail(notifications, options.Email, alertCopy);
            }

            if (options.Telegram.IsEnabled)
            {
                SendTelegramMessage(options.Telegram, alertCopy);
            }
        }

        #endregion Methods
    }
}