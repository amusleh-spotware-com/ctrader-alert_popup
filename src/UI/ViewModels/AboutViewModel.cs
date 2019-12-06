using Prism.Commands;
using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        public AboutViewModel()
        {
            Model = new Models.AboutMode();

            RequestNavigateCommand = new DelegateCommand<string>(url => System.Diagnostics.Process.Start(url));
        }

        #region Properties

        public Models.AboutMode Model { get; }

        public DelegateCommand<string> RequestNavigateCommand { get; set; }

        #endregion Properties
    }
}