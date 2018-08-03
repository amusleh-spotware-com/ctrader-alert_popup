using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        #region Fields

        private Models.OptionsModel _model;

        private List<SolidColorBrush> _colors;

        private List<Models.ThemeBaseModel> _themeBases;

        private List<Models.ThemeAccentModel> _themeAccents;

        private List<FontFamily> _fonts;

        private List<Models.FontWeightModel> _fontWeights;

        private List<Models.FontStyleModel> _fontStyles;

        private List<Enums.TimeFormat> _timeFormats;

        private List<TimeZoneInfo> _timeZones;

        #endregion Fields

        #region Constructor

        public OptionsViewModel(Models.OptionsModel model)
        {
            _model = model;

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            BrowserSoundFileCommand = new DelegateCommand(BrowserSoundFile);
        }

        #endregion Constructor

        #region Properties

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        public Models.OptionsModel Model
        {
            get
            {
                return _model;
            }
        }

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

        public List<Enums.TimeFormat> TimeFormats
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

        public DelegateCommand BrowserSoundFileCommand { get; set; }

        #endregion Properties

        #region Methods

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
        }

        private void Unloaded()
        {
        }

        private void BrowserSoundFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV files (*.wav)|*.wav";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                Model.Sound.FilePath = openFileDialog.FileName;
            }
        }

        #endregion Methods
    }
}