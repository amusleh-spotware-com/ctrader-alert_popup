using MahApps.Metro;
using MahApps.Metro.Controls;
using Nortal.Utilities.Csv;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Alert
{
    public partial class AlertWindow : MetroWindow
    {
        #region Fields

        private double normalWindowHeight;
        private double normalWindowWidth;

        private System.Timers.Timer refreshCheckTimer = new System.Timers.Timer();

        #endregion Fields

        #region Constructor

        public AlertWindow()
        {
            Loaded += MetroWindow_Loaded;
            Closing += MetroWindow_Closing;
            SizeChanged += MetroWindow_SizeChanged;

            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Alert;component/Resources/Icons.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", Manager.CurrentAccent.Name), UriKind.RelativeOrAbsolute) });
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(string.Format("pack://application:,,,/MahApps.Metro;component/Styles/Accents/{0}.xaml", Manager.CurrentTheme.Name), UriKind.RelativeOrAbsolute) });

            InitializeComponent();

            if (Properties.Settings.Default.IsWindowSizeSaved)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.Height = Properties.Settings.Default.WindowHeight;
                    this.Width = Properties.Settings.Default.WindowWidth;
                }

                this.normalWindowHeight = Properties.Settings.Default.WindowHeight;
                this.normalWindowWidth = Properties.Settings.Default.WindowWidth;
            }

            Alerts = new ObservableCollection<Alert>();

            GetAlertsFromFile();
        }

        #endregion Constructor

        #region Properties

        public ObservableCollection<Alert> Alerts { get; set; }

        public Alert SelectedAlert { get; set; }

        #endregion Properties

        #region Methods

        public void Invoke(Action action)
        {
            this.Dispatcher.BeginInvoke(action);
        }

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

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            refreshCheckTimer.Elapsed += RefreshCheckTimer_Elapsed;
            refreshCheckTimer.Interval = 1000;

            refreshCheckTimer.Start();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                Properties.Settings.Default.IsWindowSizeSaved = true;

                if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Minimized)
                {
                    Properties.Settings.Default.WindowHeight = this.normalWindowHeight;
                    Properties.Settings.Default.WindowWidth = this.normalWindowWidth;
                }
            }

            refreshCheckTimer.Stop();

            if (Manager.Mutex != null)
            {
                Manager.Mutex.Close();
            }
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal && this.Visibility == Visibility.Visible)
            {
                this.normalWindowHeight = e.NewSize.Height;
                this.normalWindowWidth = e.NewSize.Width;
            }
        }

        private void RefreshCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            refreshCheckTimer.Stop();

            if (bool.Parse(Registry.GetValue("Refresh", true)))
            {
                Invoke(new Action(() =>
                {
                    GetAlertsFromFile();
                }));

                Registry.SetValue("Refresh", false);
            }

            refreshCheckTimer.Start();
        }

        private void RemoveAllAlertsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText(Manager.FilePath, string.Empty);

                Alerts.Clear();
            }
            catch (Exception ex)
            {
                Manager.LogException(ex);
            }
        }

        private void RemoveAlertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedAlert != null)
                {
                    Alerts.Remove(SelectedAlert);

                    File.WriteAllText(Manager.FilePath, string.Empty);

                    using (StringWriter writer = new StringWriter())
                    {
                        CsvWriter csv = new CsvWriter(writer, new CsvSettings());

                        foreach (Alert alert in Alerts)
                        {
                            csv.WriteLine(alert.TradeSide, alert.Symbol, alert.TimeFrame, alert.Time.ToString(), alert.Comment);
                        }

                        File.AppendAllText(Manager.FilePath, writer.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Manager.LogException(ex);
            }
        }

        private void GetAlertsFromFile()
        {
            Alerts.Clear();

            try
            {
                using (var parser = new CsvParser(File.ReadAllText(Manager.FilePath)))
                {
                    foreach (String[] line in parser.ReadToEnd())
                    {
                        Alerts.Add(new Alert()
                        {
                            TradeSide = line[0],
                            Symbol = line[1],
                            TimeFrame = line[2],
                            Time = DateTime.Parse(line[3], CultureInfo.InvariantCulture),
                            Comment = line[4]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Manager.LogException(ex);

                File.Delete(Manager.FilePath);
                File.Create(Manager.FilePath).Close();
            }

            BringInFront();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();

            refreshCheckTimer.Stop();

            this.IsEnabled = false;

            settingsWindow.ShowDialog();

            ThemeManager.ChangeAppStyle(this, Manager.CurrentAccent, Manager.CurrentTheme);

            GetAlertsFromFile();

            this.IsEnabled = true;

            refreshCheckTimer.Start();
        }

        #endregion Methods
    }
}