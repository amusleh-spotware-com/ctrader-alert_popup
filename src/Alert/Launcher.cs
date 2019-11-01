using cAlgo.API.Alert.Helpers;
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
    internal class Launcher
    {
        #region Fields

        private App _app;

        private readonly List<AlertModel> _alerts = new List<AlertModel>();

        private DateTime _lastTriggeredAlertTime;

        private readonly List<string> _templateKeywords;

        #endregion Fields

        #region Properties

        public static Launcher Current { get; } = new Launcher();

        #endregion Properties

        public Launcher()
        {
            _templateKeywords = new List<string>
            {
                "{TriggeredBy}",
                "{Time}",
                "{Type}",
                "{Symbol}",
                "{TimeFrame}",
                "{Comment}",
                "{Price}"
            };

            AppDomain.CurrentDomain.UnhandledException += (obj, args) => Logger.LogException(args.ExceptionObject as Exception);
        }

        #region Methods

        public async void Launch(INotifications notifications, AlertModel alert, bool triggerAlerts = true, bool showPopup = true)
        {
            UpdateAlerts();

            await AlertManager.AddAlert(alert);

            _alerts.Add(alert);

            SettingsModel settings = SettingsFactory.GetSettings(Configuration.Current.SettingsFilePath);

            if (triggerAlerts)
            {
                TriggerAlerts(notifications, settings, alert);
            }

            if (showPopup)
            {
                ShowPopup(alert);
            }
        }

        public void ShowPopup()
        {
            UpdateAlerts();

            ShowPopup(null);
        }

        private void PlaySound(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            SoundPlayer soundPlayer = new SoundPlayer(path);

            soundPlayer.PlaySync();

            soundPlayer.Dispose();
        }

        private string PutObjectInTemplate(object obj, string template)
        {
            Dictionary<string, object> properties = obj.GetType().GetProperties().Where(iProperty => iProperty.GetValue(obj) != null)
                .ToDictionary(propertyInfo => propertyInfo.Name,
                propertyInfo => propertyInfo.GetValue(obj));

            foreach (string keyword in _templateKeywords)
            {
                string propertyName = RemoveKeywordBrackets(keyword);

                template = template.Replace(keyword, properties.ContainsKey(propertyName) ? properties[propertyName].ToString() : string.Empty);
            }

            return template;
        }

        private string RemoveKeywordBrackets(string word)
        {
            return new string(word.Skip(1).Take(word.Length - 2).ToArray());
        }

        private void SendEmail(INotifications notifications, EmailSettingsModel settings, AlertModel alert)
        {
            string subject = PutObjectInTemplate(alert, settings.Template.Subject);
            string body = PutObjectInTemplate(alert, settings.Template.Body);

            notifications.SendEmail(settings.Sender, settings.Recipient, subject, body);
        }

        private void SendTelegramMessage(TelegramSettingsModel settings, AlertModel alert)
        {
            string message = PutObjectInTemplate(alert, settings.MessageTemplate);

            foreach (TelegramConversation conversation in settings.Conversations)
            {
                TelegramBotClient telegramBotClient = new TelegramBotClient(conversation.BotToken);

                telegramBotClient.SendTextMessage(conversation.Id, message);
            }
        }

        private void TriggerAlerts(INotifications notifications, SettingsModel settings, AlertModel alert)
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

        private void ShowPopup(AlertModel alert)
        {
            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    TimeSpan timeSinceLastAlert = DateTime.Now - _lastTriggeredAlertTime;

                    _lastTriggeredAlertTime = DateTime.Now;

                    if (timeSinceLastAlert < TimeSpan.FromSeconds(1.5))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1.5) - timeSinceLastAlert);
                    }

                    if (_app == null)
                    {
                        _app = new App(Configuration.Current.SettingsFilePath, _alerts);

                        _app.EventAggregator.GetEvent<AlertRemovedEvent>().Subscribe(AlertRemovedEvent_Handler);

                        _app.ShellView.Dispatcher.Invoke(() =>
                        {
                            _app.ShellView.Title = Configuration.Current.Title;

                            _app.ShellView.Closed += (sender, args) =>
                            {
                                _app = null;
                            };
                        });
                    }

                    if (alert != null)
                    {
                        _app.InvokeAlertAddedEvent(alert);
                    }

                    _app.Run();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);

                    throw ex;
                }
            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();
        }

        private async void AlertRemovedEvent_Handler(AlertModel alert)
        {
            await AlertManager.RemoveAlert(alert);
        }

        private async void UpdateAlerts()
        {
            _alerts.Clear();

            var updatedAlerts = await AlertManager.GetAlerts();

            if (updatedAlerts != null)
            {
                _alerts.AddRange(updatedAlerts);
            }
        }

        #endregion Methods
    }
}