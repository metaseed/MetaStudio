using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Metaseed.Windows.Threading;
namespace Metaseed.Windows.Controls
{
    /// <summary>
    /// Interaction logic for LongWaitMessage.xaml
    /// </summary>
    internal partial class LongWaitMessage : Window
    {
        public LongWaitMessage(string _message,bool isInOtherThread)
        {
            InitializeComponent();
            this.Loaded += LongWaitMessage_Loaded;
            //
            
            //if (Application.Current.MainWindow.IsLoaded)
            //{
            //    this.Owner = Application.Current.MainWindow;
            //}
            ShowInTaskbar = false;
            if (isInOtherThread)
            {
                //progressBar.IsIndeterminate = true;
                message.Text = _message;
                this.Refresh();
                this.Show();
                this.Refresh();
            }
               
        }

        void LongWaitMessage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top -= 160;
        }


    }
}
