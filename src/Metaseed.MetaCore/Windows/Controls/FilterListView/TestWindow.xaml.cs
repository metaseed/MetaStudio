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
using System.Collections.Specialized;
using Metaseed.Common.Services;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
namespace CANStudio
{
    /// <summary>
    /// Interaction logic for Shell1.xaml
    /// </summary>
    public partial class Shell1 : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        OverrideModeCollection c = new OverrideModeCollection();
        public Shell1()
        {
            InitializeComponent(); 
            this.DataContext = c;
            c.Add(new LoggerMessage(DateTime.Now, "abcaggs", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.High, "safdeaawa"));
            c.Add(new LoggerMessage(DateTime.Now, "abssfgfgc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.Medium, "safdffa"));
            c.Add(new LoggerMessage(DateTime.Now, "abdhhhtesc", Microsoft.Practices.Prism.Logging.Category.Info, Microsoft.Practices.Prism.Logging.Priority.High, "sdsesadfafda"));
            c.Add(new LoggerMessage(DateTime.Now, "abfnnc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.None, "saffdfadfdda"));
            c.Add(new LoggerMessage(DateTime.Now, "abggfc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.None, "safadfada"));

            c.Add(new LoggerMessage(DateTime.Now, "abc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.Low, "safdeaawa"));
            c.Add(new LoggerMessage(DateTime.Now, "abc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.High, "safdffa"));
             c.Add(new LoggerMessage(DateTime.Now, "abdsc", Microsoft.Practices.Prism.Logging.Category.Info, Microsoft.Practices.Prism.Logging.Priority.Medium, "sdsesadfafda"));
             c.Add(new LoggerMessage(DateTime.Now, "abfc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.High, "saffdfadfdda"));
            c.Add(new LoggerMessage(DateTime.Now, "abc", Microsoft.Practices.Prism.Logging.Category.Debug, Microsoft.Practices.Prism.Logging.Priority.None, "safadfada"));
            c.Add(new LoggerMessage(DateTime.Now, "abfarc", Microsoft.Practices.Prism.Logging.Category.Exception, Microsoft.Practices.Prism.Logging.Priority.High, "safdfadsffda"));
            c.Add(new LoggerMessage(DateTime.Now, "abgagc", Microsoft.Practices.Prism.Logging.Category.Warn, Microsoft.Practices.Prism.Logging.Priority.High, "safddfadasa"));
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,800);
            dispatcherTimer.Start();

        }
        //  System.Windows.Threading.DispatcherTimer.Tick handler
        //
        //  Updates the current seconds display and calls
        //  InvalidateRequerySuggested on the CommandManager to force 
        //  the Command to raise the CanExecuteChanged event.
        Random random = new Random(5);
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            int i=random.Next(0, c.Count - 1);
            int r2=random.Next(0, c.Count - 1);
               LoggerMessage a = new LoggerMessage(DateTime.Now,c[i].Sender,c[i].Category,c[i].Priority, c[r2].Message);
               c.Add(a);

        }

    }
    public class OverrideModeCollection : ObservableCollection<LoggerMessage>
    {
        Dictionary<Tuple<string, Category, Priority>, LoggerMessage> _internalDic =new Dictionary<Tuple<string,Category,Priority>,LoggerMessage>();
        // ListCollectionView collectionViewSource;
        public OverrideModeCollection()
        {
            //collectionViewSource = (ListCollectionView)(CollectionViewSource.GetDefaultView(this));
        }
        protected override void ClearItems()
        {
            _internalDic.Clear();
            base.ClearItems();
        }

        public Boolean IsRelativeTime = false;
        protected override void InsertItem(int index, LoggerMessage item)
        {
            var key = Tuple.Create(item.Sender, item.Category, item.Priority);

            if (_internalDic.ContainsKey(key))
            {

                var oldItem = _internalDic[key];
                var i = this.IndexOf(oldItem);//collectionViewSource.IndexOf(_internalDic[key]);
                //var key1 = Tuple.Create((byte)1, CANOpenDataType.PDO, (UInt16)1);
                //if (key.Equals(key1))
                //{
                //    System.Diagnostics.Debugger.Break();
                //}
                this[i] = item;


            }
            else
            {


                base.InsertItem(this.Count, item);
            }
            _internalDic[key] = item;
        }
    }
}
