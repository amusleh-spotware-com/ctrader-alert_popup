using cAlgo.API.Alert.Enums;
using cAlgo.API.Alert.Factories;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media;
using TelegramBotApi;
using TelegramBotApi.Types;

namespace cAlgo.API.Alert.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        #region Fields

        private List<SolidColorBrush> _colors;
        private EventAggregator _eventAggregator;
        private List<FontFamily> _fonts;
        private List<Models.FontStyleModel> _fontStyles;
        private List<Models.FontWeightModel> _fontWeights;
        private Models.SettingsModel _model;
        private Models.TelegramConversation _telegramConversation;
        private List<Models.ThemeAccentModel> _themeAccents;
        private List<Models.ThemeBaseModel> _themeBases;
        private List<TimeFormat> _timeFormats;

        private List<TimeZoneInfo> _timeZones;

        private string _telegramErrorMessage;

        #endregion Fields

        public SettingsViewModel(Models.SettingsModel model, EventAggregator eventAggregator)
        {
            _model = model;

            _eventAggregator = eventAggregator;

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            BrowserSoundFileCommand = new DelegateCommand(BrowserSoundFile);

            ResetEmailTemplateCommand = new DelegateCommand(ResetEmailTemplate);

            SettingsChangedCommand = new DelegateCommand(SettingsChanged);

            GeneralSettingsChangedCommand = new DelegateCommand(GeneralSettingsChanged);

            AlertSettingsChangedCommand = new DelegateCommand(AlertSettingsChanged);

            SoundSettingsChangedCommand = new DelegateCommand(SoundSettingsChanged);

            EmailSettingsChangedCommand = new DelegateCommand(EmailSettingsChanged);

            TelegramSettingsChangedCommand = new DelegateCommand(TelegramSettingsChanged);

            ResetTelegramTemplateCommand = new DelegateCommand(ResetTelegramTemplate);

            RequestNavigateCommand = new DelegateCommand<string>(RequestNavigate);

            AddTelegramConversationCommand = new DelegateCommand(AddTelegramConversation);

            RemoveTelegramBotCommand = new DelegateCommand<Models.TelegramConversation>(RemoveTelegramBot);

            RemoveSelectedTelegramBotsCommand = new DelegateCommand<IList>(RemoveSelectedTelegramBots);
        }

        #region Properties

        public DelegateCommand AddTelegramConversationCommand { get; set; }
        public DelegateCommand AlertSettingsChangedCommand { get; set; }
        public DelegateCommand BrowserSoundFileCommand { get; set; }

        public List<SolidColorBrush> Colors
        {
            get
            {
                return _colors;
            }
            set
            {
                SetProperty(ref _colors, value);
            }
        }

        public DelegateCommand EmailSettingsChangedCommand { get; set; }

        public List<FontFamily> Fonts
        {
            get
            {
                return _fonts;
            }
            set
            {
                SetProperty(ref _fonts, value);
            }
        }

        public List<Models.FontStyleModel> FontStyles
        {
            get
            {
                return _fontStyles;
            }
            set
            {
                SetProperty(ref _fontStyles, value);
            }
        }

        public List<Models.FontWeightModel> FontWeights
        {
            get
            {
                return _fontWeights;
            }
            set
            {
                SetProperty(ref _fontWeights, value);
            }
        }

        public DelegateCommand GeneralSettingsChangedCommand { get; set; }
        public DelegateCommand LoadedCommand { get; set; }

        public Models.SettingsModel Model
        {
            get
            {
                return _model;
            }
        }

        public DelegateCommand SettingsChangedCommand { get; set; }
        public DelegateCommand<IList> RemoveSelectedTelegramBotsCommand { get; set; }
        public DelegateCommand<Models.TelegramConversation> RemoveTelegramBotCommand { get; set; }
        public DelegateCommand<string> RequestNavigateCommand { get; set; }
        public DelegateCommand ResetEmailTemplateCommand { get; set; }
        public DelegateCommand ResetTelegramTemplateCommand { get; set; }
        public DelegateCommand SoundSettingsChangedCommand { get; set; }

        public Models.TelegramConversation TelegramConversation
        {
            get
            {
                return _telegramConversation;
            }
            set
            {
                SetProperty(ref _telegramConversation, value);
            }
        }

        public DelegateCommand TelegramSettingsChangedCommand { get; set; }

        public List<Models.ThemeAccentModel> ThemeAccents
        {
            get
            {
                return _themeAccents;
            }
            set
            {
                SetProperty(ref _themeAccents, value);
            }
        }

        public List<Models.ThemeBaseModel> ThemeBases
        {
            get
            {
                return _themeBases;
            }
            set
            {
                SetProperty(ref _themeBases, value);
            }
        }

        public List<TimeFormat> TimeFormats
        {
            get
            {
                return _timeFormats;
            }
            set
            {
                SetProperty(ref _timeFormats, value);
            }
        }

        public List<TimeZoneInfo> TimeZones
        {
            get
            {
                return _timeZones;
            }
            set
            {
                SetProperty(ref _timeZones, value);
            }
        }

        public DelegateCommand UnloadedCommand { get; set; }

        public string TelegramErrorMessage
        {
            get
            {
                return _telegramErrorMessage;
            }
            set
            {
                SetProperty(ref _telegramErrorMessage, value);
            }
        }

        #endregion Properties

        #region Methods

        private void AddTelegramConversation()
        {
            if (string.IsNullOrEmpty(TelegramConversation.Name) || string.IsNullOrEmpty(TelegramConversation.BotToken))
            {
                TelegramErrorMessage = "You have to provide both bot token and username/channel title";

                return;
            }

            TelegramBotClient telegramBotClient = new TelegramBotClient(TelegramConversation.BotToken);

            Update[] updates;

            try
            {
                updates = telegramBotClient.GetUpdates();
            }
            catch (WebException ex)
            {
                TelegramErrorMessage = ex.Message;

                return;
            }

            Update requiredUpdate = updates.FirstOrDefault(update =>
            {
                StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;

                string username = update.Message != null ? update.Message.Chat.Username : string.Empty;

                string channelTitle = update.ChannelPost != null ? update.ChannelPost.Chat.Title : string.Empty;

                if (!string.IsNullOrEmpty(username) && TelegramConversation.Name.Equals(username, stringComparison))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(channelTitle) && TelegramConversation.Name.Equals(channelTitle, stringComparison))
                {
                    return true;
                }

                return false;
            });

            if (requiredUpdate == null)
            {
                TelegramErrorMessage = "There is no open/new conversation or chat between the bot and user/channel, please send a new test message to the bot and then try again";

                return;
            }

            TelegramConversation.Id = requiredUpdate.Message != null ? requiredUpdate.Message.Chat.Id : requiredUpdate.ChannelPost.Chat.Id;

            if (Model.Telegram.Conversations.Contains(TelegramConversation))
            {
                TelegramErrorMessage = "This conversation already added";

                return;
            }

            Model.Telegram.Conversations.Add(TelegramConversation);

            TelegramConversation = new Models.TelegramConversation();

            SettingsChangedCommand.Execute();

            TelegramSettingsChangedCommand.Execute();

            TelegramErrorMessage = string.Empty;
        }

        private void AlertSettingsChanged()
        {
            _eventAggregator.GetEvent<Events.AlertSettingsChangedEvent>().Publish(Model.Alerts);
        }

        private void BrowserSoundFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = string.IsNullOrEmpty(Model.Sound.FilePath) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Model.Sound.FilePath;

            if (openFileDialog.ShowDialog() == true)
            {
                Model.Sound.FilePath = openFileDialog.FileName;
            }
        }

        private void EmailSettingsChanged()
        {
            _eventAggregator.GetEvent<Events.EmailSettingsChangedEvent>().Publish(Model.Email);
        }

        private void GeneralSettingsChanged()
        {
            _eventAggregator.GetEvent<Events.GeneralSettingsChangedEvent>().Publish(Model.General);
        }

        private void Loaded()
        {
            ThemeBases = SettingsFactory.GetThemeBases();

            ThemeAccents = SettingsFactory.GetThemeAccents();

            Colors = SettingsFactory.GetColors();

            Fonts = SettingsFactory.GetFontFamilies();

            FontWeights = SettingsFactory.GetFontWeights();

            FontStyles = SettingsFactory.GetFontStyles();

            TimeFormats = SettingsFactory.GetTimeFormats();

            TimeZones = SettingsFactory.GetTimeZones();

            TelegramConversation = new Models.TelegramConversation();
        }

        private void SettingsChanged()
        {
            _eventAggregator.GetEvent<Events.SettingsChangedEvent>().Publish(Model);
        }

        private void RemoveSelectedTelegramBots(IList selectedItems)
        {
            selectedItems.Cast<Models.TelegramConversation>().ToList().ForEach(bot => RemoveTelegramBot(bot));
        }

        private void RemoveTelegramBot(Models.TelegramConversation bot)
        {
            if (Model.Telegram.Conversations.Contains(bot))
            {
                Model.Telegram.Conversations.Remove(bot);

                SettingsChangedCommand.Execute();
                TelegramSettingsChangedCommand.Execute();
            }
        }

        private void RequestNavigate(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        private void ResetEmailTemplate()
        {
            Model.Email.Template.Subject = Model.Email.DefaultTemplate.Subject;
            Model.Email.Template.Body = Model.Email.DefaultTemplate.Body;
        }

        private void ResetTelegramTemplate()
        {
            Model.Telegram.MessageTemplate = Model.Telegram.DefaultMessageTemplate;
        }

        private void SoundSettingsChanged()
        {
            _eventAggregator.GetEvent<Events.SoundSettingsChangedEvent>().Publish(Model.Sound);
        }

        private void TelegramSettingsChanged()
        {
            _eventAggregator.GetEvent<Events.TelegramSettingsChangedEvent>().Publish(Model.Telegram);
        }

        private void Unloaded()
        {
        }

        #endregion Methods
    }
}