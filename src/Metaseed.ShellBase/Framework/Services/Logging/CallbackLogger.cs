
using System.Diagnostics;

namespace Metaseed.MetaShell.Services
{
    using System;
    using System.Collections.Generic;

    using Catel.Logging;
    using Microsoft.Practices.Prism.Logging;

    /// <summary>
    /// A logger that holds on to log entries until a callback delegate is set, then plays back log entries and sends new log entries.
    /// </summary>
    public class CallbackLogger : LogListenerBase
    {
        #region Fields

        public CallbackLogger()
        {
#if DEBUG

#else
            IsDebugEnabled = false;
            //IgnoreCatelLogging=true;
#endif
        }
        /// <summary>
        /// The saved logs.
        /// </summary>
        private readonly Queue<LoggerMessage> savedLogs = new Queue<LoggerMessage>();

        /// <summary>
        /// The callback.
        /// </summary>
        private Action<LoggerMessage> callback;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the callback to receive logs.
        /// </summary>
        /// <value>An Action&lt;string, Category, Priority&gt; callback.</value>
        public Action<LoggerMessage> Callback
        {
            get { return callback; }
            set { callback = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(LoggerMessage message)
        {
            if (this.Callback != null)
            {
                this.Callback(message);
            }
            else
            {
                this.savedLogs.Enqueue(message);
            }
        }

        /// <summary>
        /// Replays the saved logs if the Callback has been set.
        /// </summary>
        public void ReplaySavedLogs()
        {
            if (this.Callback != null)
            {
                while (this.savedLogs.Count > 0)
                {
                    var message = this.savedLogs.Dequeue();
                    this.Callback(message);
                }
            }
        }

        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData,DateTime time)
        {
            Log(new LoggerMessage(log.TargetType.Name, logEvent, message, extraData));
            if (logEvent == LogEvent.Error)
            {
                //Debugger.Break();
            }

        }
        #endregion
    }
}