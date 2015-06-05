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

        public HostedProcessWindow()
        {
            SizeChanged += OnSizeChanged;
            Loaded += DockedProcessWindow_Loaded;
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
            get {return ProcessWindowHelper.IsWindowShown(Process.MainWindowHandle); }
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
                if (pixelWidth != (int) this.MinWidth || pixelHeight != (int) this.MinHeight)
                {
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
            if (Process==null) throw new Exception("Please set the Process property of this HostedProcessWindow object!");
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
            _hidenMenu=ProcessWindowHelper.HideMenubar(Process.MainWindowHandle);
            ProcessWindowHelper.RemoveCaptionBarAndBorder(Process.MainWindowHandle);
            var panel = new Panel();
            ProcessWindowHelper.SetParent(Process.MainWindowHandle, panel.Handle);
            var windowsFormsHost = new WindowsFormsHost { Child = panel };
            Content = windowsFormsHost;
            SizeChangedFunction(this);
            ProcessWindowHelper.ShowWindow(Process.MainWindowHandle);
        }

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
            if (!_isdisposed)
            {
                if (disposing)
                {
                    _isdisposed = true;
                    try
                    {
                        if (_iscreated && !Process.HasExited)
                        {
                            Process.Kill();
                        }
                        Process.Close();
                        Process.Dispose();
                        Process = null;
                        // Clear internal handle
                        //_hWndDocked = IntPtr.Zero;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception:" + e.Message);
                    }
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