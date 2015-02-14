using System.Reflection;
using System.Windows;
using Squirrel;

namespace Resquirrelly
{
    public partial class App : Application
    {
        public App()
        {
            RegisterSquirrelEvents();
        }

        private void RegisterSquirrelEvents()
        {
            var location = UpdateHelper.AppUpdateCheckLocation;
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            using (var mgr = new UpdateManager(location, appName, FrameworkVersion.Net45))
            {
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => mgr.CreateShortcutForThisExe(),
                    onAppUpdate: v => mgr.CreateShortcutForThisExe(),
                    onAppUninstall: v => mgr.RemoveShortcutForThisExe()
                );
            }
        }

    }
}
