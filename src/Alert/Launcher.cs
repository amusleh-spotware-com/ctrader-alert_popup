using cAlgo.API.Alert.Enums;
using cAlgo.API.Alert.Events;
using cAlgo.API.Alert.Factories;
using cAlgo.API.Alert.Models;
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
    internal static class Launcher
    {
        #region Fields

        private static App _app;

        private static readonly List<AlertModel> _alerts = new List<AlertModel>();

        private static readonly List<string> _templateKeywords = new List<string>
        {
                "{TriggeredBy}",
                "{Time}",
                "{Type}",
                "{Symbol}",
                "{TimeFrame}",
                "{Comment}",
                "{Price}"
        };

        #endregion Fields

        static Launcher()
        {
            AppDomain.CurrentDomain.UnhandledException += (obj, args) => Logger.LogException(args.ExceptionObject as Exception);
        }

        #region Methods

        public static void Launch(INotifications notifications, AlertModel alert, AlertType alertType)
        {
            UpdateAlerts();

            DataManager.AddAlerts(alert);

            _alerts.Add(alert);

            var settings = SettingsFactory.GetSettings(Configuration.Current.SettingsFilePath);

            if (alertType == AlertType.Triggers || alertType == AlertType.Popup)
            {
                TriggerAlerts(notifications, settings, alert);
            }

            if (alertType == AlertType.Popup)
            {
                ShowPopup(alert);
            }
        }

        public static void ShowPopup()
        {
            UpdateAlerts();

            ShowPopup(null);
        }

        private static void PlaySound(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SoundPlayer soundPlayer = new SoundPlayer(path);

            soundPlayer.PlaySync();

            soundPlayer.Dispose();
        }

        private static string PutObjectInTemplate(object obj, string template)
        {
            Dictionary<string, object> properties = obj.GetType().GetProperties()
                .Where(iProperty => iProperty.GetValue(obj) != null)
                .ToDictionary(propertyInfo => propertyInfo.Name,
                propertyInfo => propertyInfo.GetValue(obj));

            foreach (string keyword in _templateKeywords)
            {
                string propertyName = RemoveKeywordBrackets(keyword);

                template = template.Replace(keyword, properties.ContainsKey(propertyName) 
                    ? properties[propertyName].ToString() 
                    : string.Empty);
            }

            return template;
        }

        private static string RemoveKeywordBrackets(string word)
        {
            return new string(word.Skip(1).Take(word.Length - 2).ToArray());
        }

        private static void SendEmail(INotifications notifications, EmailSettingsModel settings, AlertModel alert)
        {
            string subject = PutObjectInTemplate(alert, settings.Template.Subject);
            string body = PutObjectInTemplate(alert, settings.Template.Body);

            notifications.SendEmail(settings.Sender, settings.Recipient, subject, body);
        }

        private static void SendTelegramMessage(TelegramSettingsModel settings, AlertModel alert)
        {
            string message = PutObjectInTemplate(alert, settings.MessageTemplate);

            foreach (TelegramConversation conversation in settings.Conversations)
            {
                TelegramBotClient telegramBotClient = new TelegramBotClient(conversation.BotToken);

                telegramBotClient.SendTextMessage(conversation.Id, message);
            }
        }

        private static void TriggerAlerts(INotifications notifications, SettingsModel settings, AlertModel alert)
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

        private static void ShowPopup(AlertModel alert)
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    using (var eventWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "AlertWindowWaitHandle"))
                    {
                        eventWaitHandle.WaitOne();

                        if (_app == null)
                        {
                            InitializeApp();
                        }

                        if (alert != null)
                        {
                            _app.InvokeAlertAddedEvent(alert);
                        }

                        eventWaitHandle.Set();
                    }

                    _app.Run();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);

                    throw;
                }
            }));

            thread.SetApartmentState(ApartmentState.STA);
            thread.CurrentCulture = CultureInfo.InvariantCulture;
            thread.CurrentUICulture = CultureInfo.InvariantCulture;

            thread.Start();
        }

        private static void AlertRemovedEvent_Handler(IEnumerable<AlertModel> alerts)
        {
            DataManager.RemoveAlerts(alerts.ToArray());
        }

        private static void UpdateAlerts()
        {
            _alerts.Clear();

            var updatedAlerts = DataManager.GetAlerts();

            if (updatedAlerts != null)
            {
                _alerts.AddRange(updatedAlerts);
            }
        }

        private static void InitializeApp()
        {
            _app = new App(Configuration.Current.SettingsFilePath, _alerts);

            _app.EventAggregator.GetEvent<AlertRemovedEvent>().Subscribe(AlertRemovedEvent_Handler);

            _app.InvokeOnWindowThread(() =>
            {
                _app.ShellView.Title = Configuration.Current.Title;

                _app.ShellView.Closed += (sender, args) =>
                {
                    _app = null;
                };
            });
        }

        #endregion Methods
    }
}