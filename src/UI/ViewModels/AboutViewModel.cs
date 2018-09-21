using Prism.Commands;
using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        #region Fields

        private Models.AboutMode _model;

        #endregion Fields

        public AboutViewModel()
        {
            RequestNavigateCommand = new DelegateCommand<string>(url => System.Diagnostics.Process.Start(url));

            _model = new Models.AboutMode();
        }

        #region Properties

        public Models.AboutMode Model
        {
            get
            {
                return _model;
            }
        }

        public DelegateCommand<string> RequestNavigateCommand { get; set; }

        #endregion Properties
    }
}