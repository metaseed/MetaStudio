using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using Metaseed.Diagnostics;
using Application = System.Windows.Application;
using Control = System.Windows.Controls.Control;
using MessageBox = System.Windows.MessageBox;
using Panel = System.Windows.Forms.Panel;
using Point = System.Drawing.Point;

namespace Metaseed.Windows.Controls
{
    public class HostedProcessWindow : ContentControl
    {
        private bool _iscreated;
        private bool _isdisposed;

        public static DependencyProperty LeftProperty = DependencyProperty.Register(
            "LeftProperty",
            typeof(int),
            typeof(HostedProcessWindow)
            );

        public static DependencyProperty TopProperty = DependencyProperty.Register(
            "TopProperty",
            typeof(int),
            typeof(HostedProcessWindow)
            );

        public HostedProcessWindow(bool autoKillHostedProcess=true)
        {
            _autoKillHostedProcess = autoKillHostedProcess;
            SizeChanged += OnSizeChanged;
            Loaded += DockedProcessWindow_Loaded;
        }

         bool _autoKillHostedProcess;

        public bool AutoKillHostedProcess
        {
            get { return _autoKillHostedProcess; }
            set { _autoKillHostedProcess = value; }
        }

        public bool Iscreated
        {
            get { return _iscreated; }
            private set { _iscreated = value; }
        }

        public Process Process { get; set; }

        public int Left
        {
            get { return (int)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public int Top
        {
            get { return (int)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        public bool IsShown
        {
            get { return ProcessWindowHelper.IsWindowShown(Process.MainWindowHandle); }
        }

        private void DockedProcessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HostMainWindow();
        }

        public void Show()
        {
            ProcessWindowHelper.ShowWindow(Process.MainWindowHandle);
        }

        public void Hide()
        {
            ProcessWindowHelper.HideWindow(Process.MainWindowHandle);
        }

        virtual protected void OnSizeChanged(object s, SizeChangedEventArgs e)
        {
            var ctrl = (Control)e.Source;
            SizeChangedFunction(ctrl);
        }
        virtual protected void SizeChangedFunction(Control control)
        {
            if (!_iscreated) return;
            if (Process.MainWindowHandle != IntPtr.Zero)
            {
                //http://stackoverflow.com/questions/3286175/how-do-i-convert-a-wpf-size-to-physical-pixels/3286419#3286419   
                var pixelWidth =
                    (int)(ActualWidth * Screen.PrimaryScreen.WorkingArea.Width / SystemParameters.WorkArea.Width);
                var pixelHeight =
                    (int)(ActualHeight * Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height);
                //exclue the unnecessary rearrangement 
                if (pixelWidth != (int)this.MinWidth || pixelHeight != (int)this.MinHeight)
                {
                    ProcessWindowHelper.MoveWindow(Process.MainWindowHandle, 0, 0, pixelWidth, pixelHeight, true);
                    //the below line is needed, if some control in hosted window using OnResize to do layout, in this case the last line is used to trigger the control's layout, the next line won't trigger, it just move the window to change its position.
                    ProcessWindowHelper.MoveWindow(Process.MainWindowHandle, 0, 0, pixelWidth, pixelHeight, true);
                }
                //var win = Window.GetWindow(control);
                //if (win != null)
                //{
                //    var handle = new WindowInteropHelper(win).Handle;
                //    HwndSource source = (HwndSource)HwndSource.FromVisual(this);
                //    IntPtr hWnd = source.Handle;
                //    //MoveWindow(handle, 0, 0, pixelWidth, pixelHeight, true);
                //}

                //old code:
                //var win = Window.GetWindow(control); // System.Windows.Application.Current.MainWindow
                //if (win != null && win.IsAncestorOf(this))
                //{
                //    var point = this.TransformToAncestor(win).Transform(new Point(this.Left, this.Top));
                //    MoveWindow(Process.MainWindowHandle, point.X, point.Y, pixelWidth, pixelHeight, true);
                //}
            }
            InvalidateVisual();
        }

        private IntPtr _hidenMenu;
        virtual protected void HostMainWindow()
        {
            if (_iscreated) return;
            _iscreated = true;
            if (Process == null) throw new Exception("Please set the Process property of this HostedProcessWindow object!");
            Process.Refresh();
            if (Process.MainWindowHandle == IntPtr.Zero)
            {
                Process.Refresh();
                Thread.Sleep(10);
                Process.Refresh();
            }
            if (Process.MainWindowHandle == IntPtr.Zero)
            {
                throw new Exception("Could not find the Process main window!");
            }
            ProcessWindowHelper.HideWindow(Process.MainWindowHandle);
            _hidenMenu = ProcessWindowHelper.HideMenubar(Process.MainWindowHandle);
            captionBorderStyleBackup=ProcessWindowHelper.RemoveCaptionBarAndBorder(Process.MainWindowHandle);
            var panel = new Panel();
            ProcessWindowHelper.SetParent(Process.MainWindowHandle, panel.Handle);
            var windowsFormsHost = new ProcessWindowHost { Child = panel };
            Content = windowsFormsHost;
            SizeChangedFunction(this);
            
            ProcessWindowHelper.ShowWindow(Process.MainWindowHandle);
            StartListeningForWindowChanges();
            Application.Current.Exit += Current_Exit;
        }

        private long captionBorderStyleBackup;
        private void Current_Exit(object sender, ExitEventArgs e)
        {
            if (!AutoKillHostedProcess)
            {
                ShowMenubar();
                ProcessWindowHelper.RecoverCaptionBarAndBorder(Process);
                ProcessWindowHelper.SetParent(Process.MainWindowHandle, IntPtr.Zero);
               
            }
        }





        #region Window Event Hook
        //public enum HookType : int
        //{
        //    WH_JOURNALRECORD = 0,
        //    WH_JOURNALPLAYBACK = 1,
        //    WH_KEYBOARD = 2,
        //    WH_GETMESSAGE = 3,
        //    WH_CALLWNDPROC = 4,
        //    WH_CBT = 5,
        //    WH_SYSMSGFILTER = 6,
        //    WH_MOUSE = 7,
        //    WH_HARDWARE = 8,
        //    WH_DEBUG = 9,
        //    WH_SHELL = 10,
        //    WH_FOREGROUNDIDLE = 11,
        //    WH_CALLWNDPROCRET = 12,
        //    WH_KEYBOARD_LL = 13,
        //    WH_MOUSE_LL = 14
        //}
        //delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        //[DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);
        //[DllImport("user32.dll")]
        //static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        //[DllImport("user32.dll")]
        //static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, uint dwflags);

        [DllImport("user32.dll")]
        internal static extern int UnhookWinEvent(IntPtr hWinEventHook);
        internal delegate void WinEventProc(IntPtr hWinEventHook, uint iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);

        const uint WINEVENT_SKIPOWNTHREAD = 0x0001; // Don't call back for events on installer's thread
        const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process
        const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        const uint EVENT_SYSTEM_FOREGROUND = 3;

        private IntPtr winHook;
        //private IntPtr winHookAcived;
        private WinEventProc listener;
        //private HookProc myCallbackDelegate = null;
        public void StartListeningForWindowChanges()
        {
            listener = new WinEventProc(EventCallback);
            //setting the window hook
            winHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, listener, Process.Id, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS | WINEVENT_SKIPOWNTHREAD);

            //this.myCallbackDelegate = new HookProc(this.MyCallbackFunction);
            //winHookAcived = SetWindowsHookEx(HookType.WH_SHELL, myCallbackDelegate, IntPtr.Zero, IntPtr.Zero);
        }

        //private int MyCallbackFunction(int code, IntPtr wParam, IntPtr lParam)
        //{
        //    if (code < 0)
        //    {
        //        //you need to call CallNextHookEx without further processing
        //        //and return the value returned by CallNextHookEx
        //        return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        //    }
        //    // we can convert the 2nd parameter (the key code) to a System.Windows.Forms.Keys enum constant

        //    //return the value returned by CallNextHookEx
        //    return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        //}

        public void StopListeningForWindowChanges()
        {
            UnhookWinEvent(winHook);
        }

        //internal const int WM_NCLBUTTONDOWN = 0xA1;
        //internal const int HT_CAPTION = 0x2;
        //[DllImportAttribute("user32.dll")]
        //internal static extern int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        private void EventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild,
            int dwEventThread, int dwmsEventTime)
        {
            if (Process.MainWindowHandle == hWnd)
            {
                if (eventType == EVENT_SYSTEM_FOREGROUND)                // handle active window changed!
                {
                    Console.WriteLine("Active" + Process.Id);

                    var e = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                    {
                        RoutedEvent = Mouse.MouseDownEvent,
                        Source = this
                    };
                    this.RaiseEvent(e);
                }
            }

        }
        #endregion

        public void ShowMenubar()
        {
            ProcessWindowHelper.ShowMenubar(Process.MainWindowHandle, _hidenMenu);
        }

        public void HideMenubar()
        {
            var menubar = ProcessWindowHelper.HideMenubar(Process.MainWindowHandle);
            if (menubar != IntPtr.Zero)
            {
                _hidenMenu = menubar;
            }
        }
        ~HostedProcessWindow()
        {
            Dispose();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_isdisposed) return;
            StopListeningForWindowChanges();
            if (disposing)
            {
                _isdisposed = true;
                try
                {
                    if (AutoKillHostedProcess)
                    {
                        if (_iscreated && !Process.HasExited)
                        {
                            Process.Kill();
                        }
                        Process.Close();
                        Process.Dispose();
                    }
                    Process = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception:" + e.Message);
                }
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}