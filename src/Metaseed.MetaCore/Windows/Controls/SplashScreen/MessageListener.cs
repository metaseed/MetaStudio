using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
namespace Metaseed.Common
{
    /// <summary>
    /// Message listener, singlton pattern.
    /// Inherit from DependencyObject to implement DataBinding.
    /// </summary>
    public class MessageListener : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        private static MessageListener mInstance;

        /// <summary>
        /// 
        /// </summary>
        private MessageListener ( )
        {
            Messages = new ObservableCollection<string>();
        }

        /// <summary>
        /// Get MessageListener instance
        /// </summary>
        public static MessageListener Instance
        {
            get
            {
                if ( mInstance == null )
                    mInstance = new MessageListener ( );
                return mInstance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveMessage ( string message )
        {
           // Splasher.Splash.Dispatcher.BeginInvoke((Action)delegate()
          //  {
                Message = message;
                Messages.Add(message);
                //Debug.WriteLine ( Message );
                DispatcherHelper.DoEvents();
           // });
        }



        public ObservableCollection<string> Messages
        {
            get { return (ObservableCollection<string>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<string>), typeof(MessageListener), new UIPropertyMetadata(null));

        

        /// <summary>
        /// Get or set received message
        /// </summary>
        public string Message
        {
            get { return ( string ) GetValue ( MessageProperty ); }
            set { SetValue ( MessageProperty, value ); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register ( "Message", typeof ( string ), typeof ( MessageListener ), new UIPropertyMetadata ( null ) );

    }
}
