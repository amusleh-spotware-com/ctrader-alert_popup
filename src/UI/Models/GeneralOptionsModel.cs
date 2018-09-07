using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class GeneralOptionsModel : BindableBase
    {
        #region Fields

        private ThemeBaseModel _themeBase;

        private ThemeAccentModel _themeAccent;

        private bool _topMost;

        #endregion Fields

        #region Properties

        public ThemeBaseModel ThemeBase
        {
            get
            {
                return _themeBase;
            }
            set
            {
                SetProperty(ref _themeBase, value);
            }
        }

        public ThemeAccentModel ThemeAccent
        {
            get
            {
                return _themeAccent;
            }
            set
            {
                SetProperty(ref _themeAccent, value);
            }
        }

        public bool TopMost
        {
            get
            {
                return _topMost;
            }
            set
            {
                SetProperty(ref _topMost, value);
            }
        }

        #endregion Properties
    }
}