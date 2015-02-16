using System;
using System.Threading;

namespace Resquirrelly
{
    /// <summary>
    /// This is the entry point for the application.
    /// </summary>
    public class StartupManager
    {
        // Randomly created using http://createguid.com :)
        private const string MutexName = "42EEFE03-B355-4C8B-99A9-DDC3FFEC8C8A";
        private static readonly Mutex InstanceMutex = new Mutex(true, MutexName);

        [STAThread]
        static void Main()
        {
            // Prevent more than one instance of the app from running at a time. 
            // Using this approach: http://stackoverflow.com/a/522874/255385

            if (InstanceMutex.WaitOne(TimeSpan.Zero, true))
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
            else
            {
                // Send a Win32 message to make the Main window of the currently 
                // running instance appear above the other windows.
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }

        }

        /// <summary>
        /// Releases the mutex that prevents multiple app instances.
        /// </summary>
        public static void Cleanup()
        {
            if (InstanceMutex != null) InstanceMutex.ReleaseMutex();
        }
    }
}
