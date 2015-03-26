using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Win32
{
    /// <summary>
    /// usage:
    /// var ht=new HighResolutionTimer(1,action);
    /// ht.Dispose();
    /// </summary>
    public class HighResolutionTimer : IDisposable
    {
        TimerQueueTimer qt;
        MMTimer mt;
        bool usingTimerQueue;
        Action _action;
        public HighResolutionTimer()
        {

        }

        public void Start(uint period, Action action)
        {
            _action = action;
            if (Environment.OSVersion.Version.Major > 5)//>xp
            {
                qt = new TimerQueueTimer();
                TimerQueueTimer.WaitOrTimerDelegate CallbackDelete = new TimerQueueTimer.WaitOrTimerDelegate(QueueTimerCallback);
                qt.Start(0, period, CallbackDelete);
                usingTimerQueue = true;
            }
            else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1)//xp
            {
                usingTimerQueue = false;
                mt = new MMTimer();
                mt.Start(period, true, _action);
            }
            else
            {
                throw new HighResolutionTimerException("Could Not Creat 1ms Timer!");
            }
        }
        public void Stop()
        {
            if (usingTimerQueue)
            {
                qt.Stop();
            }
            else
            {
                mt.Stop();
            }
        }
        private void QueueTimerCallback(IntPtr lpParam, bool TimerOrWaitFired)
        {
            _action();
        }
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
                    this.Stop();
                }
            }
            disposed = true;
        }

        ~HighResolutionTimer()
        {
            Dispose(false);
        }
    }
}
