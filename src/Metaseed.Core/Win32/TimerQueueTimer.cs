using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace Metaseed.Win32
{
    //http://social.msdn.microsoft.com/Forums/vstudio/en-US/822aed2d-dca0-4a8e-8130-20fab69557d2/high-precision-timers-in-c?forum=csharpgeneral
    /*
     usage:
     * TimerQueueTimer qt;
            qt = new TimerQueueTimer();
            TimerQueueTimer.WaitOrTimerDelegate CallbackDelete = new TimerQueueTimer.WaitOrTimerDelegate(QueueTimerCallback);
            qt.Create(uint.Parse(textBox2.Text), uint.Parse(textBox1.Text), CallbackDelete);

With a callback delegate as
private void QueueTimerCallback(IntPtr pWhat, bool success)
{
//...
}
I just have the Callback method output a stopwatch.elapsed.
     */
    public class TimerQueueTimer : IDisposable
    {

        IntPtr phNewTimer; // Handle to the timer.

        #region Win32 TimerQueueTimer Functions
        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        static extern uint timeBeginPeriod(uint uPeriod);
        [DllImport("kernel32.dll")]
        static extern bool CreateTimerQueueTimer(
            out IntPtr phNewTimer,          // phNewTimer - Pointer to a handle; this is an out value
            IntPtr TimerQueue,              // TimerQueue - Timer queue handle. For the default timer queue, NULL
            WaitOrTimerDelegate Callback,   // Callback - Pointer to the callback function
            IntPtr Parameter,               // Parameter - Value passed to the callback function
            uint DueTime,                   // DueTime - Time (milliseconds), before the timer is set to the signaled state for the first time 
            uint Period,                    // Period - Timer period (milliseconds). If zero, timer is signaled only once
            uint Flags                      // Flags - One or more of the next values (table taken from MSDN):
            // WT_EXECUTEINTIMERTHREAD 	The callback function is invoked by the timer thread itself. This flag should be used only for short tasks or it could affect other timer operations.
            // WT_EXECUTEINIOTHREAD 	The callback function is queued to an I/O worker thread. This flag should be used if the function should be executed in a thread that waits in an alertable state.

                                            // The callback function is queued as an APC. Be sure to address reentrancy issues if the function performs an alertable wait operation.
            // WT_EXECUTEINPERSISTENTTHREAD 	The callback function is queued to a thread that never terminates. This flag should be used only for short tasks or it could affect other timer operations.

                                            // Note that currently no worker thread is persistent, although no worker thread will terminate if there are any pending I/O requests.
            // WT_EXECUTELONGFUNCTION 	Specifies that the callback function can perform a long wait. This flag helps the system to decide if it should create a new thread.
            // WT_EXECUTEONLYONCE 	The timer will be set to the signaled state only once.
            );

        [DllImport("kernel32.dll")]
        static extern bool DeleteTimerQueueTimer(
            IntPtr timerQueue,              // TimerQueue - A handle to the (default) timer queue
            IntPtr timer,                   // Timer - A handle to the timer
            IntPtr completionEvent          // CompletionEvent - A handle to an optional event to be signaled when the function is successful and all callback functions have completed. Can be NULL.
            );


        [DllImport("kernel32.dll")]
        static extern bool DeleteTimerQueue(IntPtr TimerQueue);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        #endregion

        public delegate void WaitOrTimerDelegate(IntPtr lpParameter, bool timerOrWaitFired);

        public TimerQueueTimer()
        {

        }
        WaitOrTimerDelegate _callbackDelegate;
        public void Start(uint dueTime, uint period, WaitOrTimerDelegate callbackDelegate)
        {
                _callbackDelegate = callbackDelegate;
                IntPtr pParameter = IntPtr.Zero;
                timeBeginPeriod(1);
                bool success = CreateTimerQueueTimer(
                    // Timer handle
                    out phNewTimer,
                    // Default timer queue. IntPtr.Zero is just a constant value that represents a null pointer.
                    IntPtr.Zero,
                    // Timer callback function
                    _callbackDelegate,
                    // Callback function parameter
                    pParameter,
                    // Time (milliseconds), before the timer is set to the signaled state for the first time.
                    dueTime,
                    // Period - Timer period (milliseconds). If zero, timer is signaled only once.
                    period,
                    (uint)Flag.WT_EXECUTEINIOTHREAD);

                if (!success)
                    throw new HighResolutionTimerException("Error creating QueueTimer, HighResolutionTimer");
        }

        public void Stop()
        {
            //bool success = DeleteTimerQueue(IntPtr.Zero);
            bool success = DeleteTimerQueueTimer(
                IntPtr.Zero, // TimerQueue - A handle to the (default) timer queue
                phNewTimer,  // Timer - A handle to the timer
                IntPtr.Zero  // CompletionEvent - A handle to an optional event to be signaled when the function is successful and all callback functions have completed. Can be NULL.
                );
            int error = Marshal.GetLastWin32Error();
            _callbackDelegate = null;
            //CloseHandle(phNewTimer);
        }

        private enum Flag
        {
            WT_EXECUTEDEFAULT = 0x00000000,
            WT_EXECUTEINIOTHREAD = 0x00000001,
            //WT_EXECUTEINWAITTHREAD       = 0x00000004,
            WT_EXECUTEONLYONCE = 0x00000008,
            WT_EXECUTELONGFUNCTION = 0x00000010,
            WT_EXECUTEINTIMERTHREAD = 0x00000020,
            WT_EXECUTEINPERSISTENTTHREAD = 0x00000080,
            //WT_TRANSFER_IMPERSONATION    = 0x00000100
        }

        #region IDisposable Members
        //IDisposable code
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //queen timer
                    Stop();
                }
            }
            disposed = true;
        }

        ~TimerQueueTimer()
        {
            Dispose(false);
        }


        #endregion
    }

    public class HighResolutionTimerException : Exception
    {
        public HighResolutionTimerException(string message)
            : base(message)
        {
        }

        public HighResolutionTimerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
