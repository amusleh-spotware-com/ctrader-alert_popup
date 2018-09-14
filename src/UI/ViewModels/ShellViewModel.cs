using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Commands;
using MahApps.Metro;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Fields

        private Bootstrapper _bootstrapper;

        private bool _isOptionsButtonEnabled;

        #endregion Fields

        

        public ShellViewModel(Bootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            LoadedCommand = new DelegateCommand(Loaded);

            UnloadedCommand = new DelegateCommand(Unloaded);

            GoBackCommand = new DelegateCommand(GoBack, () => _bootstrapper.NavigationJournal.Count > 1);
        }

        

        #region Properties

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand UnloadedCommand { get; set; }

        public DelegateCommand<string> NavigateCommand { get; set; }

        public bool IsOptionsButtonEnabled
        {
            get
            {
                return _isOptionsButtonEnabled;
            }
            set
            {
                SetProperty(ref _isOptionsButtonEnabled, value);
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
            if (viewName.Equals(ViewNames.OptionsView))
            {
                IsOptionsButtonEnabled = false;
            }
            else
            {
                IsOptionsButtonEnabled = true;
            }

            _bootstrapper.Navigate(viewName);

            GoBackCommand.RaiseCanExecuteChanged();
        }

        private void GoBack()
        {
            string lastNavigatedViewName = _bootstrapper.NavigationJournal.LastOrDefault();

            if (string.IsNullOrEmpty(lastNavigatedViewName))
            {
                return;
            }

            if (lastNavigatedViewName.Equals(_bootstrapper.CurrentView, StringComparison.InvariantCultureIgnoreCase))
            {
                _bootstrapper.NavigationJournal.Remove(lastNavigatedViewName);
            }

            string previousView = _bootstrapper.NavigationJournal.LastOrDefault();

            _bootstrapper.NavigationJournal.Remove(previousView);

            Navigate(previousView);
        }

        #endregion Methods
    }
}