using cAlgo.API.Alert.Factories;
using cAlgo.API.Alert.Models;
using cAlgo.API.Alert.UI;
using cAlgo.API.Alert.UI.Events;
using cAlgo.API.Alert.UI.Factories;
using cAlgo.API.Alert.UI.Models;
using cAlgo.API.Alert.Utility;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Threading;
using TelegramBotApi;

namespace cAlgo.API.Alert
{
    public class Launcher
    {
        #region Fields

        private static Launcher _current = new Launcher();

        private App _app;

        private List<AlertModel> _alerts;

        private Logger _logger;

        #endregion Fields

        public Launcher()
        {
            _logger = new Logger(Configuration.Current.Tracer);
        }

        #region Properties

        public static Launcher Current => _current;

        #endregion Properties

        #region Methods

        public void PlaySound(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SoundPlayer soundPlayer = new SoundPlayer(path);

            soundPlayer.PlaySync();

            soundPlayer.Dispose();
        }

        public string PutObjectInTemplate(object obj, string template)
        {
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

            List<string> keywords = template.Split().Where(word => word.StartsWith("{", comparison) && word.EndsWith("}", comparison)).ToList();

            Dictionary<string, object> properties = obj.GetType().GetProperties().ToDictionary(propertyInfo => propertyInfo.Name,
                propertyInfo => propertyInfo.GetValue(obj));

            foreach (string keyword in keywords)
            {
                string propertyName = RemoveKeywordBrackets(keyword);

                if (properties.ContainsKey(propertyName))
                {
                    template = template.Replace(keyword, properties[propertyName].ToString());
                }
            }

            return template;
        }

        public string RemoveKeywordBrackets(string word)
        {
            return new string(word.Skip(1).Take(word.Length - 2).ToArray());
        }

        public void SendEmail(INotifications notifications, EmailSettingsModel settings, AlertModel alert)
        {
            string subject = PutObjectInTemplate(alert, settings.Template.Subject);
            string body = PutObjectInTemplate(alert, settings.Template.Body);

            notifications.SendEmail(settings.Sender, settings.Recipient, subject, body);
        }

        public void SendTelegramMessage(TelegramSettingsModel settings, AlertModel alert)
        {
            string message = PutObjectInTemplate(alert, settings.MessageTemplate);

            foreach (TelegramConversation conversation in settings.Conversations)
            {
                TelegramBotClient telegramBotClient = new TelegramBotClient(conversation.BotToken);

                telegramBotClient.SendTextMessage(conversation.Id, message);
            }
        }

        public void TriggerAlerts(INotifications notifications, SettingsModel settings, AlertModel alert)
        {
            if (settings.Sound.IsEnabled)
            {
                PlaySound(settings.Sound.FilePath);
            }

            AlertModel alertCopy = alert.Clone() as AlertModel;

            alertCopy.Price = Math.Round(alertCopy.Price, settings.Alerts.MaxPriceDecimalPlacesNumber);

            if (!alertCopy.Time.Offset.Equals(settings.Alerts.TimeZone.BaseUtcOffset))
            {
                alertCopy.Time = alert.Time.ToOffset(settings.Alerts.TimeZone.BaseUtcOffset);
            }

            if (settings.Email.IsEnabled)
            {
                SendEmail(notifications, settings.Email, alertCopy);
            }

            if (settings.Telegram.IsEnabled)
            {
                SendTelegramMessage(settings.Telegram, alertCopy);
            }
        }

        public void Launch(INotifications notifications, AlertModel alert)
        {
            _alerts = AlertsFactory.GetAlerts().ToList();

            AlertsFactory.AddAlert(alert);

            _alerts.Add(alert);

            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                SettingsModel settings = SettingsFactory.GetSettings(Configuration.Current.SettingsFilePath);

                TriggerAlerts(notifications, settings, alert);

                if (_app == null)
                {
                    _app = new App(Configuration.Current.SettingsFilePath, _alerts);

                    _app.EventAggregator.GetEvent<AlertRemovedEvent>().Subscribe(AlertRemovedEvent_Handler);

                    _app.ShellView.Title = Configuration.Current.Title;

                    _app.ShellView.Closed += (sender, args) =>
                    {
                        _app.EventAggregator.GetEvent<AlertRemovedEvent>().Unsubscribe(AlertRemovedEvent_Handler);

                        _app = null;
                    };
                }

                _app.InvokeAlertAddedEvent(alert);

                _app.Run();
            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();
        }

        private void AlertRemovedEvent_Handler(AlertModel alert)
        {
            AlertsFactory.RemoveAlert(alert);
        }

        #endregion Methods
    }
}