using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Fields

        private App _app;

        private bool _isSettingsButtonEnabled;

        #endregion Fields

        public ShellViewModel(App app)
        {
            _app = app;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            GoBackCommand = new DelegateCommand(GoBack, () => _app.NavigationJournal.Count > 1);
        }

        #region Properties

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        public DelegateCommand<string> NavigateCommand { get; set; }

        public bool IsSettingsButtonEnabled
        {
            get
            {
                return _isSettingsButtonEnabled;
            }
            set
            {
                SetProperty(ref _isSettingsButtonEnabled, value);
            }
        }

        public DelegateCommand GoBackCommand { get; set; }

        #endregion Properties

        #region Methods

        private void Loaded()
        {
            Navigate(ViewNames.AlertsView);
        }

        private void Unloaded()
        {
        }

        private void Navigate(string viewName)
        {
            if (viewName.Equals(ViewNames.SettingsView))
            {
                IsSettingsButtonEnabled = false;
            }
            else
            {
                IsSettingsButtonEnabled = true;
            }

            _app.Navigate(viewName);

            GoBackCommand.RaiseCanExecuteChanged();
        }

        private void GoBack()
        {
            string lastNavigatedViewName = _app.NavigationJournal.LastOrDefault();

            if (string.IsNullOrEmpty(lastNavigatedViewName))
            {
                return;
            }

            if (lastNavigatedViewName.Equals(_app.CurrentView, StringComparison.InvariantCultureIgnoreCase))
            {
                _app.NavigationJournal.Remove(lastNavigatedViewName);
            }

            string previousView = _app.NavigationJournal.LastOrDefault();

            _app.NavigationJournal.Remove(previousView);

            Navigate(previousView);
        }

        #endregion Methods
    }
}