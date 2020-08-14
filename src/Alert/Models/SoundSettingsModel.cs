using Prism.Mvvm;

namespace cAlgo.API.Alert.Models
{
    public class SoundSettingsModel : BindableBase
    {
        #region Fields

        private bool _isEnabled;

        private string _filePath;

        #endregion Fields

        #region Properties

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                SetProperty(ref _isEnabled, value);
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                SetProperty(ref _filePath, value);
            }
        }

        #endregion Properties
    }
}