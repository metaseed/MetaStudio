using System;

namespace Metaseed
{
    /// <summary>
    ///  Designed so it operates in a very similar way to the System.Timers.Timer class,
    ///  it has a timer interval in microseconds and Start / Stop methods (or Enabled property).
    ///  The timer implements a custom event handler (MicroTimerElapsedEventHandler) that fires every interval. 
    ///  The NotificationTimer function is where the 'work' is done and is run in a separate high priority thread.
    ///  It should be noted that MicroTimer is inefficient and very processor hungry 
    ///  as the NotificationTimer function runs a tight while loop until the elapsed microseconds
    ///  is greater than the next interval. The while loop uses a SpinWait, 
    ///  this is not a sleep but runs for a few nanoseconds and effectively 
    ///  puts the thread to sleep without relinquishing the remainder of its CPU time slot. 
    ///  This is not ideal; however, for such small intervals,
    ///  this is probably the only practical solution.
    ///  
    /// This derives from and extends the System.Diagnostics.Stopwatch class; 
    /// importantly, it provides the additional property ElapsedMicroseconds. 
    /// This is useful as a standalone class where the elapsed microseconds 
    /// from when the stopwatch was started can be directly obtained.
    /// </summary>
    public class MicroStopwatch : System.Diagnostics.Stopwatch
    {
        readonly double _microSecPerTick =
            1000000D / System.Diagnostics.Stopwatch.Frequency;

        public MicroStopwatch()
        {
            if (!System.Diagnostics.Stopwatch.IsHighResolution)
            {
                throw new Exception("On this system the high-resolution " +
                                    "performance counter is not available");
            }
        }

        public long ElapsedMicroseconds
        {
            get
            {
                return (long)(ElapsedTicks * _microSecPerTick);
            }
        }
    }

    /// <summary>
    /// MicroTimer is designed for situations were a very quick timer is required (around the 1ms mark); 
    /// however, due to the non real-time nature of the Windows Operating System, it can never be accurate. 
    /// However, as no other microsecond software timers are available, 
    /// it does offer a reasonable solution for this task 
    /// (and although processor hungry, is reasonably accurate on fast systems).
    /// </summary>
    public class MicroTimer
    {
        public delegate void MicroTimerElapsedEventHandler(
                             object sender,
                             MicroTimerEventArgs timerEventArgs);
        public event MicroTimerElapsedEventHandler MicroTimerElapsed;

        System.Threading.Thread _threadTimer = null;
        long _ignoreEventIfLateBy = long.MaxValue;
        long _timerIntervalInMicroSec = 0;
        bool _stopTimer = true;

        public MicroTimer()
        {
        }

        public MicroTimer(long timerIntervalInMicroseconds)
        {
            Interval = timerIntervalInMicroseconds;
        }

        public long Interval
        {
            get
            {
                return System.Threading.Interlocked.Read(
                    ref _timerIntervalInMicroSec);
            }
            set
            {
                System.Threading.Interlocked.Exchange(
                    ref _timerIntervalInMicroSec, value);
            }
        }
        /// <summary>
        /// By default, MicroTimer will always try and catch up on the next interval.
        /// The advantage of this is the number of times the OnTimeEvent is called will
        /// always be correct for the total elapsed time 
        /// (which is why the OnTimedEvent must take significantly less time than the interval; 
        /// if it takes a similar or longer time, 
        /// MicroTimer can never 'catch up' and the timer event will always be late). 
        /// The disadvantage of this is when it's trying to 'catch up', 
        /// the actual interval achieved will be much less than the required interval as
        /// the callback function is called in quick succession in an attempt to catch up.
        /// 
        /// whereby the callback function (OnTimedEvent) will not be called if the timer 
        /// is late by the specified number of microseconds.
        /// The advantage of this is the timer will not attempt to 'catch up',
        /// i.e., it will not call the callback function in quick succession in an attempt to catch up. 
        /// The disadvantage is that some events will be missed.
        /// </summary>
        public long IgnoreEventIfLateBy
        {
            get
            {
                return System.Threading.Interlocked.Read(
                    ref _ignoreEventIfLateBy);
            }
            set
            {
                System.Threading.Interlocked.Exchange(
                    ref _ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value);
            }
        }

        public bool Enabled
        {
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
            get
            {
                return (_threadTimer != null && _threadTimer.IsAlive);
            }
        }

        public void Start()
        {
            if (Enabled || Interval <= 0)
            {
                return;
            }

            _stopTimer = false;

            System.Threading.ThreadStart threadStart = delegate()
            {
                NotificationTimer(ref _timerIntervalInMicroSec,
                                  ref _ignoreEventIfLateBy,
                                  ref _stopTimer);
            };

            _threadTimer = new System.Threading.Thread(threadStart);
            _threadTimer.Priority = System.Threading.ThreadPriority.Highest;
            _threadTimer.Start();
        }
        /// <summary>
        /// This method stops the timer by setting a flag to instruct the timer to stop,
        /// however, this call executes asynchronously 
        /// i.e. the call to Stop will return immediately 
        /// (but the current timer event may not have finished).
        /// </summary>
        public void Stop()
        {
            _stopTimer = true;
        }

        /// <summary>
        /// This method stops the timer synchronously, 
        /// it will not return until the current timer (callback) event has finished
        /// and the timer thread has terminated. 
        /// </summary>
        public void StopAndWait()
        {
            StopAndWait(System.Threading.Timeout.Infinite);
        }
        /// <summary>
        /// StopAndWait also has an overload method that accepts a timeout (in ms), 
        /// if the timer successfully stops within the timeout period then true is returned, else false is returned.
        /// </summary>
        /// <param name="timeoutInMilliSec"></param>
        /// <returns></returns>
        public bool StopAndWait(int timeoutInMilliSec)
        {
            _stopTimer = true;

            if (!Enabled || _threadTimer.ManagedThreadId ==
                System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                return true;
            }

            return _threadTimer.Join(timeoutInMilliSec);
        }
        /// <summary>
        /// This method may be used as a last resort to terminate the timer thread, 
        /// for example, to abort the timer if it has not stopped after waiting 1sec (1000ms) 
        /// use:if( !microTimer.StopAndWait(1000) ){ microTimer.Abort(); }
        /// </summary>
        public void Abort()
        {
            _stopTimer = true;

            if (Enabled)
            {
                _threadTimer.Abort();
            }
        }

        void NotificationTimer(ref long timerIntervalInMicroSec,
                               ref long ignoreEventIfLateBy,
                               ref bool stopTimer)
        {
            int timerCount = 0;
            long nextNotification = 0;

            MicroStopwatch microStopwatch = new MicroStopwatch();
            microStopwatch.Start();

            while (!stopTimer)
            {
                long callbackFunctionExecutionTime =
                    microStopwatch.ElapsedMicroseconds - nextNotification;

                long timerIntervalInMicroSecCurrent =
                    System.Threading.Interlocked.Read(ref timerIntervalInMicroSec);
                long ignoreEventIfLateByCurrent =
                    System.Threading.Interlocked.Read(ref ignoreEventIfLateBy);

                nextNotification += timerIntervalInMicroSecCurrent;
                timerCount++;
                long elapsedMicroseconds = 0;

                while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds)
                        < nextNotification)
                {
                    System.Threading.Thread.SpinWait(10);
                }

                long timerLateBy = elapsedMicroseconds - nextNotification;

                if (timerLateBy >= ignoreEventIfLateByCurrent)
                {
                    continue;
                }

                MicroTimerEventArgs microTimerEventArgs =
                     new MicroTimerEventArgs(timerCount,
                                             elapsedMicroseconds,
                                             timerLateBy,
                                             callbackFunctionExecutionTime);
                MicroTimerElapsed(this, microTimerEventArgs);
            }

            microStopwatch.Stop();
        }
    }

    /// <summary>
    /// MicroTimer Event Argument class
    /// Derived from System.EventArgs, this class provides an object for holding information about the event.
    /// Namely, the number of times the event has fired, the absolute time (in microseconds) 
    /// from when the timer was started, how late the event was and the execution time of the
    /// callback function (for the previous event). From this data, a range of timer information can be derived.
    /// </summary>
    public class MicroTimerEventArgs : EventArgs
    {
        // Simple counter, number times timed event (callback function) executed
        public int TimerCount { get; private set; }

        // Time when timed event was called since timer started
        public long ElapsedMicroseconds { get; private set; }

        // How late the timer was compared to when it should have been called
        public long TimerLateBy { get; private set; }

        // Time it took to execute previous call to callback function (OnTimedEvent)
        public long CallbackFunctionExecutionTime { get; private set; }

        public MicroTimerEventArgs(int timerCount,
                                   long elapsedMicroseconds,
                                   long timerLateBy,
                                   long callbackFunctionExecutionTime)
        {
            TimerCount = timerCount;
            ElapsedMicroseconds = elapsedMicroseconds;
            TimerLateBy = timerLateBy;
            CallbackFunctionExecutionTime = callbackFunctionExecutionTime;
        }
    }
}
