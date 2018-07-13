using System;
using System.Windows;

namespace cAlgo.API.Alert.UI
{
    public class Bootstrapper
    {
        #region Fields

        private readonly string _theme, _accent;

        private readonly Views.ShellView _shellView;

        #endregion Fields

        #region Constructors

        public Bootstrapper(string theme, string accent)
        {
            _theme = theme;

            _accent = accent;

            _shellView = CreateView<Views.ShellView>(this);
        }

        #endregion Constructors

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

            (view as FrameworkElement).Resources.MergedDictionaries.Add(GetMetroResource(_theme));
            (view as FrameworkElement).Resources.MergedDictionaries.Add(GetMetroResource(_accent));

            return view;
        }

        #endregion Methods
    }
}