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
using System.Windows.Threading;
namespace Metaseed.Common
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class MySplashScreen : Window
    {
        //DispatcherTimer dispatcherTimer;
        public MySplashScreen ( )
        {
            InitializeComponent ( );
            //dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0,0,0,0,300);
            //dispatcherTimer.Start();
            //TextEffectRotateTransform.CenterX =(Canvas.GetLeft(taiji) +taiji.Width )/ 2;
            //TextEffectRotateTransform.CenterY =(Canvas.GetTop(taiji)  +taiji.Height )/ 2;
//            ((System.Collections.ObjectModel.ObservableCollection<string>)(lb.Items.SourceCollection)).CollectionChanged += (s, e) => {  
//   var sv = /* find ScrollViewer in lb's visual tree */ lb as ScrollViewer;  
//   sv.ScrollToEnd();  
//};  
        }

        //http://social.msdn.microsoft.com/Forums/en/wpf/thread/0f524459-b14e-4f9a-8264-267953418a2d
        private void m_cStatusList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange > 0.0)
                ((ScrollViewer)e.OriginalSource).ScrollToEnd();
        }
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    DispatcherHelper.DoEvents();
        //}
       public bool IsShowScrollBar {
            get {return  ScrollViewer.GetVerticalScrollBarVisibility(lb)==ScrollBarVisibility.Hidden; }
            set
            {
                ScrollViewer.SetVerticalScrollBarVisibility(lb, ScrollBarVisibility.Auto);
            }
        }
    }
}
