using MahApps.Metro;
using Prism.Mvvm;
using System;
using System.Windows.Media;

namespace cAlgo.API.Alert.UI.Models
{
    public class GeneralOptionsModel : BindableBase
    {
        #region Fields

        private ThemeBaseModel _themeBase;

        private ThemeAccentModel _themeAccent;

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

        #endregion Properties
    }
}