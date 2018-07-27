using Prism.Commands;
using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        #region Fields

        private Models.OptionsModel _model;

        #endregion Fields

        #region Constructor

        public OptionsViewModel(Models.OptionsModel model)
        {
            _model = model;

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);
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