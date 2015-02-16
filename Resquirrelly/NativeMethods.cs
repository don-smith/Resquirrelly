using System;
using System.Runtime.InteropServices;

namespace Resquirrelly
{
    class NativeMethods
    {        
        // The following code is used to setup Win32 messaging so we can handle
        // messages from subsequent instances of this app that try to start. They
        // won't be able to start because of the mutex registered in StartupManager.cs
        // but they will send a message before they exit. The current instance will
        // respond to that message and show the Main window as a result.

        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}
