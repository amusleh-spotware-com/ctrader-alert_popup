using cAlgo.API.Alert.UI.Types.Enums;
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
using Telegram.Bot;
using Telegram.Bot.Types;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        #region Fields

        private List<SolidColorBrush> _colors;
        private EventAggregator _eventAggregator;
        private List<FontFamily> _fonts;
        private List<Models.FontStyleModel> _fontStyles;
        private List<Models.FontWeightModel> _fontWeights;
        private Models.OptionsModel _model;
        private Models.TelegramConversation _telegramConversation;
        private List<Models.ThemeAccentModel> _themeAccents;
        private List<Models.ThemeBaseModel> _themeBases;
        private List<TimeFormat> _timeFormats;

        private List<TimeZoneInfo> _timeZones;

        private string _telegramErrorMessage;

        #endregion Fields

        public OptionsViewModel(Models.OptionsModel model, EventAggregator eventAggregator)
        {
            _model = model;

            _eventAggregator = eventAggregator;

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            BrowserSoundFileCommand = new DelegateCommand(BrowserSoundFile);

            ResetEmailTemplateCommand = new DelegateCommand(ResetEmailTemplate);

            OptionsChangedCommand = new DelegateCommand(OptionsChanged);

            GeneralOptionsChangedCommand = new DelegateCommand(GeneralOptionsChanged);

            AlertOptionsChangedCommand = new DelegateCommand(AlertOptionsChanged);

            SoundOptionsChangedCommand = new DelegateCommand(SoundOptionsChanged);

            EmailOptionsChangedCommand = new DelegateCommand(EmailOptionsChanged);

            TelegramOptionsChangedCommand = new DelegateCommand(TelegramOptionsChanged);

            ResetTelegramTemplateCommand = new DelegateCommand(ResetTelegramTemplate);

            RequestNavigateCommand = new DelegateCommand<string>(RequestNavigate);

            AddTelegramConversationCommand = new DelegateCommand(AddTelegramConversation);

            RemoveTelegramBotCommand = new DelegateCommand<Models.TelegramConversation>(RemoveTelegramBot);

            RemoveSelectedTelegramBotsCommand = new DelegateCommand<IList>(RemoveSelectedTelegramBots);
        }

        #region Properties

        public DelegateCommand AddTelegramConversationCommand { get; set; }
        public DelegateCommand AlertOptionsChangedCommand { get; set; }
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

        public DelegateCommand EmailOptionsChangedCommand { get; set; }

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

        public DelegateCommand GeneralOptionsChangedCommand { get; set; }
        public DelegateCommand LoadedCommand { get; set; }

        public Models.OptionsModel Model
        {
            get
            {
                return _model;
            }
        }

        public DelegateCommand OptionsChangedCommand { get; set; }
        public DelegateCommand<IList> RemoveSelectedTelegramBotsCommand { get; set; }
        public DelegateCommand<Models.TelegramConversation> RemoveTelegramBotCommand { get; set; }
        public DelegateCommand<string> RequestNavigateCommand { get; set; }
        public DelegateCommand ResetEmailTemplateCommand { get; set; }
        public DelegateCommand ResetTelegramTemplateCommand { get; set; }
        public DelegateCommand SoundOptionsChangedCommand { get; set; }

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

        public DelegateCommand TelegramOptionsChangedCommand { get; set; }

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

            OptionsChangedCommand.Execute();

            TelegramOptionsChangedCommand.Execute();

            TelegramErrorMessage = string.Empty;
        }

        private void AlertOptionsChanged()
        {
            _eventAggregator.GetEvent<Events.AlertOptionsChangedEvent>().Publish(Model.Alerts);
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

        private void EmailOptionsChanged()
        {
            _eventAggregator.GetEvent<Events.EmailOptionsChangedEvent>().Publish(Model.Email);
        }

        private void GeneralOptionsChanged()
        {
            _eventAggregator.GetEvent<Events.GeneralOptionsChangedEvent>().Publish(Model.General);
        }

        private void Loaded()
        {
            ThemeBases = OptionsBaseViewModel.GetThemeBases();

            ThemeAccents = OptionsBaseViewModel.GetThemeAccents();

            Colors = OptionsBaseViewModel.GetColors();

            Fonts = OptionsBaseViewModel.GetFontFamilies();

            FontWeights = OptionsBaseViewModel.GetFontWeights();

            FontStyles = OptionsBaseViewModel.GetFontStyles();

            TimeFormats = OptionsBaseViewModel.GetTimeFormats();

            TimeZones = OptionsBaseViewModel.GetTimeZones();

            TelegramConversation = new Models.TelegramConversation();
        }

        private void OptionsChanged()
        {
            _eventAggregator.GetEvent<Events.OptionsChangedEvent>().Publish(Model);
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

                OptionsChangedCommand.Execute();
                TelegramOptionsChangedCommand.Execute();
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

        private void SoundOptionsChanged()
        {
            _eventAggregator.GetEvent<Events.SoundOptionsChangedEvent>().Publish(Model.Sound);
        }

        private void TelegramOptionsChanged()
        {
            _eventAggregator.GetEvent<Events.TelegramOptionsChangedEvent>().Publish(Model.Telegram);
        }

        private void Unloaded()
        {
        }

        #endregion Methods
    }
}