using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;

namespace Resquirrelly
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ScheduleAppUpdates();
        }

        public async Task ScheduleAppUpdates()
        {
            const int appUpdateInterval = 20000; // 20 seconds
            var appUpdateTimer = new Timer(ScheduleApplicationUpdates, null, 0, appUpdateInterval);
            await appUpdateTimer.Start();
        }

        private async void ScheduleApplicationUpdates(Object o)
        {
            var location = UpdateHelper.AppUpdateCheckLocation;
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            using (var mgr = new UpdateManager(location, appName, FrameworkVersion.Net45))
            {
                try
                {
                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.FutureReleaseEntry != null)
                    {
                        if (updateInfo.CurrentlyInstalledVersion == updateInfo.FutureReleaseEntry) return;
                        await mgr.UpdateApp();
                        Dispatcher.Invoke(ShowUpdateIsAvailable);
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                }
            }
        }

        private async void RestartButtonClicked(object sender, RoutedEventArgs e)
        {
            UpdateManager.RestartApp();
            await Task.Delay(1000);
            Application.Current.Shutdown(0);
        }

        private void ShowUpdateIsAvailable()
        {
            RestartButton.Visibility = Visibility.Visible;
            Instructions.Visibility = Visibility.Hidden;
        }

    }

}
