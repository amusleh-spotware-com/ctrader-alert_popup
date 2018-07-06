using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace cAlgo.API.Alert.UI.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        #region Fields

        private Bootstrapper _bootstrapper;

        #endregion Fields

        #region Constructor

        public ShellViewModel(Bootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;

            NavigateCommand = new DelegateCommand(Navigate, viewName => !string.IsNullOrEmpty(viewName.ToString()));
        }

        #endregion Constructor

        #region Properties

        public bool IsClosed { get; set; }

        public DelegateCommand NavigateCommand { get; set; }

        #endregion Properties

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Methods

        private void BringInFront()
        {
            if (this.Visibility == Visibility.Hidden || this.Visibility == Visibility.Collapsed)
            {
                this.Visibility = Visibility.Visible;
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartPipeListener();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsClosed = true;
        }

        private void SendCloseWindowsMessage()
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(Properties.Settings.Default.PipeName))
                {
                    pipeClient.Connect(1000);

                    pipeClient.WriteByte(1);
                }
            }
            catch
            {
            }
        }

        private void StartPipeListener()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    while (!IsClosed)
                    {
                        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(
                            Properties.Settings.Default.PipeName,
                            PipeDirection.InOut,
                            NamedPipeServerStream.MaxAllowedServerInstances))
                        {
                            pipeServer.WaitForConnection();

                            int result = pipeServer.ReadByte();

                            switch (result)
                            {
                                case 1:
                                    {
                                        Factory.CloseWindow();

                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Factory.LogException(ex);
                }
            }));

            thread.CurrentCulture = CultureInfo.InvariantCulture;
            thread.CurrentUICulture = CultureInfo.InvariantCulture;

            thread.Start();
        }

        private void Navigate(object viewName)
        {
            _bootstrapper.Navigate(viewName.ToString());
        }

        #endregion Methods
    }
}