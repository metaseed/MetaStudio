using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Metaseed.Windows.Threading;
using System.ComponentModel;
/*
 *usage:
 *1.    same thread:
 *  LongWaitMessager lwm = new LongWaitMessager("I'am Connecting The Network, Please Waiting...",false/true);
 *  do something...
 *  lwm.Close();
 *  lwm = null;
*/
namespace Metaseed.Windows.Controls
{
    /// <summary>
    /// show a top message window to the user
    /// </summary>
    public class LongWaitMessager:INotifyPropertyChanged
    {
        LongWaitMessage lwm;
        LongWaitMessage lwm_A;
        Timer InCaseNotCloseProperlyTimer;
        public LongWaitMessager(string message):this(message,true)
        {
            //InCaseNotCloseProperlyTimer = new Timer(InCaseNotCloseProperlyTimer_Callback, null, 60000, Timeout.Infinite);

        }
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        double _ProgressValue;
        public double ProgressValue {
            get { return _ProgressValue; }
            set
            {
                if (value != _ProgressValue)
                {
                    _ProgressValue = value;
                    NotifyPropertyChanged("ProgressValue");
                }
            }
        }
        string _Message;
        public string Message
        {
            get { return _Message; }
            set
            {
                if (!value.Equals( _Message))
                {
                    _Message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }
        void InCaseNotCloseProperlyTimer_Callback(Object o)
        {
            
            this.Close();
            if (InCaseNotCloseProperlyTimer != null)
            {
                InCaseNotCloseProperlyTimer.Dispose();
                InCaseNotCloseProperlyTimer = null;
            }
        }
        public void Close()
        {
            LongWaitMessage lwmC=null;
            if (_IsInAnotherUIThread)
            {
                lwmC = lwm_A;
            }
            else
            {
                if (lwm==null)
                {
                    Thread.Sleep(1000);
                }
                lwmC = lwm;
            }
            if (lwmC != null)
            {
                lwmC.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if (lwmC != null)
                    {
                        lwmC.Close();
                        lwmC = null;
                    }
                })
                );

            }
            //close.Set();
            isOpen = false;
        }
        private EventWaitHandle latch = new EventWaitHandle(false, EventResetMode.AutoReset);
        //private EventWaitHandle close = new EventWaitHandle(false, EventResetMode.AutoReset);
        volatile bool isOpen = false;
        Thread newWindowThread;
        //http://msdn.microsoft.com/en-us/library/ms741870.aspx
        
        bool _IsInAnotherUIThread;
        public LongWaitMessager(string message, bool isInAnotherUIThread)
        {
            _Message = message;
            _IsInAnotherUIThread = isInAnotherUIThread;

        }
        public void Show(){
            if (_IsInAnotherUIThread)
            {
                isOpen = true;
                Dispatcher dispatcher = System.Windows.Application.Current.MainWindow.Dispatcher;
                 newWindowThread = new Thread(new ThreadStart(() =>
                {
                    latch.Set();
                    lwm_A = new LongWaitMessage(_Message, _IsInAnotherUIThread);
                    lwm_A.DataContext = this;
                    while (isOpen)
                    {
                        //dispatcher.BeginInvoke(new Action(() => System.Windows.Application.Current.MainWindow.Refresh()));
                        lwm_A.Refresh();
                        Thread.Sleep(500);
                    }
                    //EventWaitHandle.WaitAny(new WaitHandle[] { close });
                    lwm_A.Close();
                }));
                 newWindowThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
                EventWaitHandle.WaitAny(new WaitHandle[] { latch });//wait thread start
            }
            else
            {
                lwm = new LongWaitMessage(_Message, _IsInAnotherUIThread);
                lwm.DataContext = this;
                lwm.ShowDialog();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
