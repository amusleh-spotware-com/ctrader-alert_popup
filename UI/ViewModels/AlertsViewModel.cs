using Prism.Commands;
using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class AlertsViewModel : BindableBase
    {
        #region Fields

        private Bootstrapper _bootstrapper;

        #endregion Fields

        #region Constructor

        public AlertsViewModel(Bootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);
        }

        #endregion Constructor

        #region Properties

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        #endregion Properties

        #region Methods

        private void Loaded()
        {
        }

        private void Unloaded()
        {
        }

        #endregion Methods
    }
}