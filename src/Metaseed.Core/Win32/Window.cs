using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//http://stackoverflow.com/questions/2719756/find-window-with-specific-text-for-a-process
using System.Windows.Forms.VisualStyles;

namespace Metaseed.Win32
{

    public static class ProcessExtension
    {
        public static bool MainWindowTitle(this Process process,Func<string, bool> compareTitle)
        {
            var ptr = WindowExt.FindWindowInProcess(process, compareTitle);
            return ptr != IntPtr.Zero;
        }
    }   

    /// <summary>
    /// IntPtr hWnd = FindWindowInProcess(p, s => s.EndsWith("ABC"));
    /// if (hWnd != IntPtr.Zero) 
    /// {
    /// // The window was found....
    /// }
    /// </summary>
    public class WindowExt
    {
        public static IntPtr FindWindowInProcess(Process process, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            Console.WriteLine("Count:"+process.Threads.Count);
            foreach (ProcessThread t in process.Threads)
            {
                Console.WriteLine(t.Id);
                windowHandle = FindWindowInThread_WithFirtLevelChildren(t.Id, compareTitle);
                if (windowHandle != IntPtr.Zero)
                {
                    break;
                }
            }

            return windowHandle;
        }

        private static IntPtr FindWindowInThread(int threadId, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            EnumThreadWindows(threadId, (hWnd, lParam) =>
            {
                var text = new StringBuilder(200);
                GetWindowText(hWnd, text, 200);
                if (compareTitle(text.ToString()))
                {
                    windowHandle = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return windowHandle;
        }


        private static IntPtr FindWindowInThread_WithFirtLevelChildren(int threadId, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            EnumThreadWindows(threadId, (hWnd, lParam) =>
            {
                var text = new StringBuilder(200);
                GetWindowText(hWnd, text, 200);
                Console.WriteLine("--"+text);
                if (compareTitle(text.ToString()))
                {
                    windowHandle = hWnd;
                    return false;
                }
                else
                {
                    windowHandle = FindChildWindow(hWnd, compareTitle);
                    if (windowHandle != IntPtr.Zero)
                    {
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);

            

            return windowHandle;
        }

        private static IntPtr FindChildWindow(IntPtr hWnd, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            EnumChildWindows(hWnd, (hChildWnd, lParam) =>
            {
                var text = new StringBuilder(200);
                GetWindowText(hChildWnd, text, 200);
                Console.WriteLine("child:"+text);
                if (compareTitle(text.ToString()))
                {
                    windowHandle = hChildWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return windowHandle;
        }

        #region InterOp
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool EnumThreadWindows(int threadId, EnumWindowsProc callback, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);
        #endregion

    }
}
