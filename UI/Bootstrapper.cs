using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Alert.UI
{
    public class Bootstrapper
    {
        #region Fields

        private readonly Views.ShellView _shellView;

        private string _currentView;

        private readonly List<string> _navigationJournal;

        #endregion Fields

        #region Constructors

        public Bootstrapper()
        {
            _shellView = CreateView<Views.ShellView>(this);

            _navigationJournal = new List<string>();
        }

        #endregion Constructors

        #region Properties

        public Window ShellView
        {
            get
            {
                return _shellView;
            }
        }

        public string CurrentView
        {
            get
            {
                return _currentView;
            }
        }

        public List<string> NavigationJournal
        {
            get
            {
                return _navigationJournal;
            }
        }

        #endregion Properties

        #region Methods

        public void Run()
        {
            _shellView.ShowDialog();
        }

        public void Shutdown()
        {
            _shellView.Close();
        }

        public void Navigate(string viewName)
        {
            if (viewName.Equals(_currentView, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            _currentView = viewName;

            _navigationJournal.Add(_currentView);

            switch (viewName)
            {
                case ViewNames.AlertsView:
                    _shellView.Content = CreateView<Views.AlertsView>(this);
                    break;

                case ViewNames.OptionsView:
                    _shellView.Content = CreateView<Views.OptionsView>(this);
                    break;
            }
        }

        private ResourceDictionary GetMetroResource(string name)
        {
            Uri uri = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", name));

            return new ResourceDictionary() { Source = uri };
        }

        private T GetViewModel<T>(params object[] parameters) where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), parameters);
        }

        private T CreateView<T>(params object[] parameters) where T : class
        {
            T view = (T)Activator.CreateInstance(typeof(T));

            (view as FrameworkElement).DataContext = GetViewModel<ViewModels.ShellViewModel>(parameters);

            (view as FrameworkElement).Resources.MergedDictionaries.Add(GetMetroResource("Blue"));
            (view as FrameworkElement).Resources.MergedDictionaries.Add(GetMetroResource("BaseDark"));

            return view;
        }

        #endregion Methods
    }
}