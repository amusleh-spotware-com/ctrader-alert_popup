using cAlgo.API.Alert.Types.Enums;
using cAlgo.API.Alert.UI;
using cAlgo.API.Alert.UI.Models;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using Telegram.Bot;

namespace cAlgo.API.Alert.Types
{
    public static class Controller
    {
        #region Fields

        private static Mutex _mutex;

        private static Bootstrapper _bootstrapper;

        #endregion Fields

        static Controller()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        #region Methods

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
                ExceptionLogger.LogException(ex);
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
                ExceptionLogger.LogException(ex);
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
                ExceptionLogger.LogException(ex);
            }
        }

        public static void SetConfigurationIfNotYet()
        {
            Configuration.Tracer = Configuration.Tracer ?? new Action<string>(message => System.Diagnostics.Trace.WriteLine(message));

            if (!string.IsNullOrEmpty(Configuration.AlertFilePath) && !string.IsNullOrEmpty(Configuration.OptionsFilePath))
            {
                return;
            }

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string calgoDirPath = Path.Combine(documentsPath, "cAlgo");

            if (!Directory.Exists(calgoDirPath))
            {
                Directory.CreateDirectory(calgoDirPath);
            }

            Configuration.AlertFilePath = Configuration.AlertFilePath ?? Path.Combine(calgoDirPath, "Alerts.csv");
            Configuration.OptionsFilePath = Configuration.OptionsFilePath ?? Path.Combine(calgoDirPath, "PopupOptions.xml");
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

        public static void Show(this INotifications notifications, AlertModel alert)
        {
            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    OptionsModel options = Bootstrapper.GetOptions(Configuration.OptionsFilePath);

                    TriggerAlerts(notifications, options, alert);

                    if (IsMutexNew() || !IsPipeServerAlive())
                    {
                        StartPipeServer();

                        _bootstrapper = new Bootstrapper(Configuration.AlertFilePath, Configuration.OptionsFilePath, options);

                        _bootstrapper.AddAlert(alert);

                        _bootstrapper.Run();
                    }
                    else
                    {
                        SendAlertToPipeServer(alert);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                }
            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                ExceptionLogger.LogException(e.ExceptionObject as Exception);
            }
        }

        private static void RunBootstrapper(AlertModel alert, OptionsModel options)
        {
            Thread windowThread = new Thread(new ThreadStart(() =>
            {
                _bootstrapper = new Bootstrapper(Configuration.AlertFilePath, Configuration.OptionsFilePath, options);

                _bootstrapper.AddAlert(alert);

                _bootstrapper.Run();
            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.CurrentCulture = CultureInfo.InvariantCulture;
            windowThread.CurrentUICulture = CultureInfo.InvariantCulture;

            windowThread.Start();
        }

        private static bool IsMutexNew()
        {
            bool isNew;

            _mutex = new Mutex(true, Properties.Settings.Default.MutexName, out isNew);

            return isNew;
        }

        private static bool IsPipeServerAlive()
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", Properties.Settings.Default.PipeName, PipeDirection.InOut))
            {
                try
                {
                    pipeClient.Connect(1000);

                    return true;
                }
                catch (TimeoutException)
                {
                    return false;
                }
            }
        }

        private static void SendAlertToPipeServer(AlertModel alert)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", Properties.Settings.Default.PipeName, PipeDirection.InOut))
            {
                pipeClient.Connect();

                byte[] data = GetPipePacketInBytes(PipePacketType.Alert, alert);

                pipeClient.Write(data, 0, data.Count());
            }
        }

        private static byte[] GetPipePacketInBytes<T>(PipePacketType packetType, T dataObject)
        {
            PipePacket packet = new PipePacket { PacketType = packetType, Data = Serializer.Serialize<T>(dataObject) };

            string xml = Serializer.Serialize(packet);

            return Encoding.UTF8.GetBytes(xml);
        }

        private static void StartPipeServer()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(Properties.Settings.Default.PipeName,
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances))
                    {
                        pipeServer.WaitForConnection();

                        StartPipeServer();

                        Thread.Sleep(1000);

                        byte[] readBuffer = new byte[1024];

                        pipeServer.Read(readBuffer, 0, readBuffer.Count());

                        if (readBuffer.Any(bufferByte => bufferByte != new byte()))
                        {
                            string readBufferString = Encoding.UTF8.GetString(readBuffer);

                            PipePacket pipePacket = Serializer.Derserialize<PipePacket>(readBufferString);

                            ExecutePipePacket(pipePacket);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException || ex is IOException || ex is ObjectDisposedException)
                    {
                        Thread.Sleep(3000);

                        StartPipeServer();
                    }

                    ExceptionLogger.LogException(ex);
                }
            }));

            thread.CurrentCulture = CultureInfo.InvariantCulture;
            thread.CurrentUICulture = CultureInfo.InvariantCulture;

            thread.Start();
        }

        private static void ExecutePipePacket(PipePacket pipePacket)
        {
            switch (pipePacket.PacketType)
            {
                case PipePacketType.Alert:
                    AlertModel alertModel = Serializer.Derserialize<AlertModel>(pipePacket.Data);

                    _bootstrapper.AddAlert(alertModel);

                    break;

                default:
                    throw new InvalidDataException("Unknown pipe packet type");
            }
        }

        #endregion Methods
    }
}