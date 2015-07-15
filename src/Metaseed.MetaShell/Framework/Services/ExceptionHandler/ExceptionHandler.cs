using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows;
//using System.Windows.Forms;
using System.Deployment.Application;
using Metaseed.Windows.Controls;

using Microsoft.Practices.Prism.Modularity;
using Catel.Windows;
using Catel;
namespace Metaseed.MetaShell.Services
{
    
    /// <summary>Enumerated type that defines how users will be notified of exceptions</summary>
    public enum NotificationType
    {
        /// <summary>Users will not be notified, exceptions will be automatically logged to the registered loggers</summary>
        Silent,
        /// <summary>Users will be notified an exception has occurred, exceptions will be automatically logged to the registered loggers</summary>
        Inform,
        /// <summary>Users will be notified an exception has occurred and will be asked if they want the exception logged</summary>
        Ask
    }

    /// <summary>
    /// Abstract class for logging errors to different output devices, primarily for use in Windows Forms applications
    /// </summary>
    public abstract class LoggerImplementation
    {
        /// <summary>Logs the specified error.</summary>
        /// <param name="error">The error to log.</param>
        public abstract void LogError(string error);
    }

    /// <summary>
    /// Class to log unhandled exceptions
    /// </summary>
    public class ExceptionHandler : IExceptionHandler
    {
        public event EventHandler SaveOnException;
        //public static ExceptionLogger Logger = new ExceptionLogger();
        //static ExceptionLogger() {
        //    Logger.AddLogger(new TextFileLogger());
        //    Logger.AddLogger(new EmailLogger());
        //}
        /*
         You can trap unhandled exceptions at three levels:

        AppDomain.UnhandledException From all threads in the AppDomain.
        Dispatcher.UnhandledException From a single specific UI dispatcher thread.
        Application.DispatcherUnhandledException From the main UI dispatcher thread in your WPF application.
        You should consider what level you need to trap unhandled exceptions at.

        Deciding between the last two depends upon whether you're using more than one WPF thread. This is quite an exotic situation and if you're unsure whether you are or not, then it's most likely that you're not.
         */
        /// <summary>
        /// Creates a new instance of the ExceptionLogger class
        /// </summary>
        public ExceptionHandler()
        {
            //Application.ThreadException +=new System.Threading.ThreadExceptionEventHandler(OnThreadException);
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
            loggers = new List<LoggerImplementation>();

        }
        bool handled = false;
        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (handled)
            {
                   return;
            }
            handled = true;
            HandleException(e.Exception);
            
        }

        private List<LoggerImplementation> loggers;
        /// <summary>
        /// Adds a logger implementation to the list of used loggers.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        public void AddLogger(LoggerImplementation logger)
        {
            loggers.Add(logger);
        }

        private NotificationType notificationType = NotificationType.Inform;
        /// <summary>
        /// Gets or sets the type of the notification shown to the end user.
        /// </summary>
        public NotificationType NotificationType
        {
            get { return notificationType; }
            set { notificationType = value; }
        }

        delegate void LogExceptionDelegate(Exception e);
        int counter = 0;
        static internal ManualResetEventSlim textLoggerEvent = new System.Threading.ManualResetEventSlim();
        static internal ManualResetEventSlim emailLoggerEvent = new System.Threading.ManualResetEventSlim();
        //internal static LongWaitMessager lwm;
        public void HandleException(Exception e)
        {

            LogExceptionDelegate logDelegate = new LogExceptionDelegate(LogException);
            if (e is System.Reflection.ReflectionTypeLoadException)
            {
                var typeLoadException = e as System.Reflection.ReflectionTypeLoadException;
                foreach (var item in typeLoadException.LoaderExceptions)
                {
                    MessageBox.Show(item.Message.ToString()); MessageBox.Show(item.Message.ToString());
                }
            }
            switch (notificationType)
            {
                case NotificationType.Ask:
                    if (MessageBox.Show("An unexpected error occurred - " + e.Message +
                    ". Do you wish to log the error?", "Error", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                    logDelegate.BeginInvoke(e, new AsyncCallback(LogCallBack), null);
                    break;
                case NotificationType.Inform:
                    if (counter == 0)
                    {
                        counter++;
                        logDelegate.BeginInvoke(e, new AsyncCallback(LogCallBack), null);
                        System.Windows.MessageBox.Show("An unexpected error occurred -\n " + e.Message+"\n\n"+e.StackTrace);
                        //System.Windows.MessageBox.Show("An unexpected error occurred -\n " + e.Message + "\n\n" + e.StackTrace);
                        Thread.Sleep(3);//capture the screen
                        //lwm = new LongWaitMessager("I'am Try To Process The Exception,Please Waiting...");
                        //lwm.Show();
                        PleaseWaitHelper.Show("I'am Try To Process The Exception,Please Waiting...");
                    }
                    else
                    {
                        textLoggerEvent.Wait(20000);
                        emailLoggerEvent.Wait(20000);
                        //if (lwm!=null)
                        //{
                        //    lwm.Close();
                        //}
                        PleaseWaitHelper.Hide();
                        System.Windows.MessageBox.Show("We Are Sorry To Show This Message To You,\nWe Will Solve The Problem As Soon As Possible, Thanks! \n");
                        Application.Current.Shutdown();
                    }

                    break;
                case NotificationType.Silent:
                    logDelegate.BeginInvoke(e, new AsyncCallback(LogCallBack), null);
                    break;
            }
            SaveOnException.SafeInvoke(this);
        }

        // Event handler that will be called when an unhandled
        // exception is caught
        private void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Log the exception to a file
            HandleException(e.Exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (handled)
            {
                return;
            }
            handled = true;
            HandleException((Exception)e.ExceptionObject);
            
        }

        private void LogCallBack(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            LogExceptionDelegate logDelegate = (LogExceptionDelegate)asyncResult.AsyncDelegate;
            if (!asyncResult.EndInvokeCalled)
            {
                logDelegate.EndInvoke(result);
            }

        }


        private Boolean CheckForUpdateDue()
        {
            Boolean isUpdateDue = false;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                TimeSpan updateInterval = DateTime.Now - ad.TimeOfLastUpdateCheck;
                if (updateInterval.Days > 3)
                {
                    isUpdateDue = true;
                }
            }

            return (isUpdateDue);
        }
        public Boolean IsNewVersionAvailable()
        {
            Boolean isRestartRequired = false;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                if (ad.UpdatedVersion > ad.CurrentVersion)
                {
                    isRestartRequired = true;
                }
            }

            return (isRestartRequired);
        }





        //ICallBackLoggerFacade logger = null;
        //[Import(AllowRecomposition = false)]
        //private ICallBackLoggerFacade Logger
        //{
        //    set { logger = value; }
        //}
        //;
        /// <summary>writes exception details to the registered loggers</summary>
        /// <param name="exception">The exception to log.</param>
        public void LogException(Exception exception)
        {
            StringBuilder error = ExceptionMessage.BuildExceptionMessage(exception);

            ILoggerListener_Info logger = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ILoggerListener_Info>();
            if (logger != null)
            {
                foreach (var item in logger.Messages)
                {
                    error.AppendLine(item.ToString());
                }
            }
            for (int i = 0; i < loggers.Count; i++)
            {
                loggers[i].LogError(error.ToString());
            }


        }
    }
}


