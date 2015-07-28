using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Point = System.Drawing.Point;
namespace Metaseed.Diagnostics
{
    public static class ProcessWindowHelper
    {
        public static Process StartProcess(string processName, string arguments)
        {
            var pDocked = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(processName, arguments)
                {
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = false
                    //WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            pDocked.Start();
            return pDocked;
        }
        /// <summary>
        /// start process and wait the started parameter to be true
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="arguments"></param>
        /// <param name="started"></param>
        /// <param name="hideMainWindow"></param>
        /// <param name="removeMenubar"></param>
        /// <param name="removeCaptionAndBorder"></param>
        /// <returns></returns>
        public static Process StartProcess(string processName, string arguments, ref bool started,
            bool hideMainWindow = true, bool removeMenubar = true, bool removeCaptionAndBorder = true)
        {
            try
            {
                Process process = StartProcess(processName, arguments);
                //wait and process message queue to make ui responsive
                while (!started)
                {
                    Thread.Sleep(10);
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
                }
                IntPtr hWnd = process.MainWindowHandle;
                ShowWindow(hWnd, hideMainWindow ? WindowShowStyle.Hide : WindowShowStyle.Show);
                if (removeMenubar)
                    HideMenubar(hWnd);
                if (removeMenubar)
                    RemoveCaptionBarAndBorder(hWnd);
                return process;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return null;
            }
        }

        //[DllImport("user32.dll")]
        //static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        //public const int LWA_ALPHA = 0x2;
        //public const int LWA_COLORKEY = 0x1;

        public static Process StartProcess(string processName, string arguments, bool hasSplashScreen,
            bool hideMainWindow = true, bool removeMenubar = false, bool removeCaptionAndBorder = false)
        {
            try
            {
                IntPtr hWnd = IntPtr.Zero;
                Process pDocked = StartProcess(processName, arguments);

                if (hasSplashScreen)
                {
                    // Wait for splash screen to be created and enter idle condition
                    while (hWnd == IntPtr.Zero)
                    {
                        pDocked.WaitForInputIdle(1000); //wait for the window to be ready for input;
                        pDocked.Refresh(); //update process info
                        if (pDocked.HasExited)
                        {
                            return null;
                        }
                        hWnd = pDocked.MainWindowHandle; //cache the window handle
                    }
                    //wait for splash screen to be closed(hWnd==IntPtr.Zero)
                    IntPtr hWndOld = hWnd;
                    while (hWnd == hWndOld)
                    {
                        Thread.Sleep(50);
                        pDocked.WaitForInputIdle(1000);
                        pDocked.Refresh();
                        hWnd = pDocked.MainWindowHandle;
                    }
                }

                while (hWnd == IntPtr.Zero)
                {
                    pDocked.WaitForInputIdle(1000); //wait for the window to be ready for input;
                    pDocked.Refresh(); //update process info
                    if (pDocked.HasExited)
                    {
                        return null;
                    }
                    hWnd = pDocked.MainWindowHandle; //cache the window handle
                    //Console.WriteLine(hWndDocked);                 
                }
                //RECT rct;
                //if (GetWindowRect(hWnd, out rct))
                //{
                //    pDocked.StartInfo.EnvironmentVariables["HasWindowPosition"] = true.ToString();
                //    pDocked.StartInfo.EnvironmentVariables["WindowPosition"] = true.ToString();
                //}
                //else
                //{
                //    pDocked.StartInfo.EnvironmentVariables["HasWindowPosition"] = false.ToString();

                //}
                customerizeWindow(pDocked, hWnd,hideMainWindow, removeMenubar, removeCaptionAndBorder);
                return pDocked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            return null;
        }

        internal static void customerizeWindow(Process pDocked,IntPtr hWnd,bool hideMainWindow, bool removeMenubar, bool removeCaptionAndBorder)
        {
            if (!pDocked.StartInfo.EnvironmentVariables.ContainsKey("WindowStyle"))
            {
                long style = GetWindowLong(hWnd, GWL_STYLE);
                pDocked.StartInfo.EnvironmentVariables["WindowStyle"] = style.ToString();
                if ((style & WS_MAXIMIZE) == WS_MAXIMIZE)
                {
                    //It's maximized
                }
                else if ((style & WS_MINIMIZE) == WS_MINIMIZE)
                {
                    //It's minimized
                    ShowWindow(hWnd, WindowShowStyle.ShowMaximized);
                }
            }
            if (!pDocked.StartInfo.EnvironmentVariables.ContainsKey("WindowExtStyle"))
            {
                var extStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                if ((extStyle & (WS_EX_LAYERED)) == WS_EX_LAYERED)
                {
                    SetWindowLong(hWnd, GWL_EXSTYLE, (IntPtr) (extStyle & (~WS_EX_LAYERED)));
                }
                pDocked.StartInfo.EnvironmentVariables["WindowExtStyle"] = extStyle.ToString();
            }
            //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);
            ShowWindow(hWnd, hideMainWindow ? WindowShowStyle.Hide : WindowShowStyle.Show); //need
            var hmenu = GetMenu(hWnd);
            pDocked.StartInfo.EnvironmentVariables["MenuBarHandle"] = hmenu.ToString();
            if (removeMenubar)
            {
               HideMenubar(hWnd);
            }
            if (removeCaptionAndBorder)
                RemoveCaptionBarAndBorder(hWnd);
        }

        const UInt32 WS_MINIMIZE = 0x20000000;
        const UInt32 WS_MAXIMIZE = 0x1000000;
        public static Process StartProcess(string processName, string arguments, bool hasSplashScreen, string mainWindowTile, bool hideMainWindow = true)
        {
            var process = FindProcessAndWindow(processName, mainWindowTile);
            if (process != null)
            {
                ShowWindow(process.MainWindowHandle, hideMainWindow ? WindowShowStyle.Hide : WindowShowStyle.Show);
                return process;
            }
            process = StartProcess(processName, arguments, hasSplashScreen, hideMainWindow);
            return process;
        }

        /// <summary>
        ///     use p.MainWindowHandle to get the handle
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="mainWindowTile"></param>
        /// <returns></returns>
        public static Process FindProcessAndWindow(string processName, string mainWindowTile)
        {
            if (!String.IsNullOrEmpty(mainWindowTile))
            {
                return
                    Process.GetProcesses()
                        .Where(p => processName.Contains(p.ProcessName))
                        .FirstOrDefault(p => p.MainWindowTitle.Equals(mainWindowTile));
            }
            return null;
        }

        public static void HideMenubar1(IntPtr hWndDocked)
        {
            var hmenu = GetMenu(hWndDocked);
            var count = GetMenuItemCount(hmenu);
            for (var i = 0; i < count; i++)
                RemoveMenu(hmenu, 0, (MF_BYPOSITION | MF_REMOVE));
            DrawMenuBar(hWndDocked);
        }


        public static IntPtr HideMenubar(IntPtr hWndDocked)
        {
            var hmenu = GetMenu(hWndDocked);
            SetMenu(hWndDocked, IntPtr.Zero);
            return hmenu;
        }
        public static bool ShowMenubar(IntPtr hWndDocked, IntPtr hMenu)
        {
            return SetMenu(hWndDocked, hMenu);
        }

        public static long RemoveCaptionBarAndBorder(IntPtr hWndDocked)
        {
            // Remove border and what not
            long styleBackup = GetWindowLong(hWndDocked, GWL_STYLE);
            long style = styleBackup & ~WS_CAPTION & ~WS_THICKFRAME;
            // Removes Caption bar and the sizing border
            SetWindowLong(hWndDocked, GWL_STYLE, (IntPtr)style);
            return (styleBackup);
        }

        public static void RecoverCaptionBarAndBorder(Process process,IntPtr hWnd)
        {
            //long styleBackup = GetWindowLong(hWndDocked, GWL_STYLE);
            //long style = styleBackup | WS_CAPTION | WS_THICKFRAME;
            var styleString=process.StartInfo.EnvironmentVariables["WindowStyle"];
            if (string.IsNullOrEmpty(styleString)) return;
            var style = long.Parse(styleString);
            SetWindowLong(hWnd, GWL_STYLE, (IntPtr)(style));
            var extStyleString = process.StartInfo.EnvironmentVariables["WindowExtStyle"];
            if (string.IsNullOrEmpty(extStyleString)) return;
            var styleExt = long.Parse(extStyleString);
            SetWindowLong(hWnd, GWL_EXSTYLE, (IntPtr)(styleExt));
        }

        public static bool IsWindowShown(IntPtr hWnd)
        {
            //below: not work showCmd is alwary ShowNormal
            //WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            //placement.length = Marshal.SizeOf(placement);
            //GetWindowPlacement(Process.MainWindowHandle, ref placement);
            //return placement.showCmd != 0;
            //Console.WriteLine(Process.MainWindowTitle);
            long style = GetWindowLong(hWnd, GWL_STYLE);
            return (style & WS_VISIBLE) != 0;
        }

        public static void ShowWindow(IntPtr hWnd)
        {
            //Console.WriteLine(Process.MainWindowTitle);
            ShowWindow(hWnd, WindowShowStyle.Show);
        }
        public static void HideWindow(IntPtr hWnd)
        {
            //Console.WriteLine(Process.MainWindowTitle);
            ShowWindow(hWnd, WindowShowStyle.Hide);
        }

        #region InterOp

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }



        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        internal const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
        internal const int WS_CHILD = 0x40000000;
        internal const uint WS_POPUP = 0x80000000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        private static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        internal static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern int SetWindowLong([In] IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
            int wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        #endregion

        #region ShowWindow

        //const UInt32 WS_VISIBLE = 0x10000000;
        /// <summary>Shows a Window</summary>
        /// <remarks>
        ///     <para>
        ///         To perform certain special effects when showing or hiding a
        ///         window, use AnimateWindow.
        ///     </para>
        ///     <para>
        ///         The first time an application calls ShowWindow, it should use
        ///         the WinMain function's nCmdShow parameter as its nCmdShow parameter.
        ///         Subsequent calls to ShowWindow must use one of the values in the
        ///         given list, instead of the one specified by the WinMain function's
        ///         nCmdShow parameter.
        ///     </para>
        ///     <para>
        ///         As noted in the discussion of the nCmdShow parameter, the
        ///         nCmdShow value is ignored in the first call to ShowWindow if the
        ///         program that launched the application specifies startup information
        ///         in the structure. In this case, ShowWindow uses the information
        ///         specified in the STARTUPINFO structure to show the window. On
        ///         subsequent calls, the application must call ShowWindow with nCmdShow
        ///         set to SW_SHOWDEFAULT to use the startup information provided by the
        ///         program that launched the application. This behavior is designed for
        ///         the following situations:
        ///     </para>
        ///     <list type="">
        ///         <item>
        ///             Applications create their main window by calling CreateWindow
        ///             with the WS_VISIBLE flag set.
        ///         </item>
        ///         <item>
        ///             Applications create their main window by calling CreateWindow
        ///             with the WS_VISIBLE flag cleared, and later call ShowWindow with the
        ///             SW_SHOW flag set to make it visible.
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">
        ///     Specifies how the window is to be shown.
        ///     This parameter is ignored the first time an application calls
        ///     ShowWindow, if the program that launched the application provides a
        ///     STARTUPINFO structure. Otherwise, the first time ShowWindow is called,
        ///     the value should be the value obtained by the WinMain function in its
        ///     nCmdShow parameter. In subsequent calls, this parameter can be one of
        ///     the WindowShowStyle members.
        /// </param>
        /// <returns>
        ///     If the window was previously visible, the return value is nonzero.
        ///     If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        /// <summary>
        ///     Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        ///     A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        ///     A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
        ///     <para>
        ///         Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails
        ///         if lpwndpl-> length is not set correctly.
        ///     </para>
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     <para>
        ///         If the function fails, the return value is zero. To get extended error information, call GetLastError.
        ///     </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int flags;
            public int length;
            public Point ptMaxPosition;
            public Point ptMinPosition;
            public Rectangle rcNormalPosition;
            public WindowShowStyle showCmd;
        }

        /// <summary>
        ///     Enumeration of the different ways of showing a window using
        ///     ShowWindow
        /// </summary>
        internal enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,

            /// <summary>
            ///     Activates and displays a window. If the window is minimized
            ///     or maximized, the system restores it to its original size and
            ///     position. An application should specify this flag when displaying
            ///     the window for the first time.
            /// </summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,

            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,

            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,

            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,

            /// <summary>
            ///     Displays a window in its most recent size and position.
            ///     This value is similar to "ShowNormal", except the window is not
            ///     actived.
            /// </summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,

            /// <summary>
            ///     Activates the window and displays it in its current size
            ///     and position.
            /// </summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,

            /// <summary>
            ///     Minimizes the specified window and activates the next
            ///     top-level window in the Z order.
            /// </summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,

            /// <summary>
            ///     Displays the window as a minimized window. This value is
            ///     similar to "ShowMinimized", except the window is not activated.
            /// </summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,

            /// <summary>
            ///     Displays the window in its current size and position. This
            ///     value is similar to "Show", except the window is not activated.
            /// </summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,

            /// <summary>
            ///     Activates and displays the window. If the window is
            ///     minimized or maximized, the system restores it to its original size
            ///     and position. An application should specify this flag when restoring
            ///     a minimized window.
            /// </summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,

            /// <summary>
            ///     Sets the show state based on the SW_ value specified in the
            ///     STARTUPINFO structure passed to the CreateProcess function by the
            ///     program that started the application.
            /// </summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,

            /// <summary>
            ///     Windows 2000/XP: Minimizes a window, even if the thread
            ///     that owns the window is hung. This flag should only be used when
            ///     minimizing windows from a different thread.
            /// </summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        #endregion

        #region menu bar

        public static uint MF_BYPOSITION = 0x400;
        public static uint MF_REMOVE = 0x1000;

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMenu(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

        [DllImport("user32.dll")]
        private static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        private static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        #endregion
    }
}
