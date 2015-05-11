using System.Collections.Specialized;
using Fluent;

using System.ComponentModel;
using System.Windows;
using Catel.Logging;
using System.Diagnostics;
using Catel.IoC;
namespace Metaseed.MetaShell.Controls
{
    using Metaseed.Views;
    using Services;
    public class RibbonGroupBoxContextUI : RibbonGroupBox, IContextUI, INotifyPropertyChanged
    {
        static readonly ILog Log = LogManager.GetCurrentClassLogger();
        static protected Ribbon Ribbon;
        bool _HasInitialized = false;
        public bool HasInitialized { get { return _HasInitialized; } }
        string _RibbonTabName = null;
        public string RibbonTabName { get { return _RibbonTabName; } set { _RibbonTabName = value; } }
        public virtual void Initialize()
        {
            Debug.Assert(Name != null);
            Debug.Assert(RibbonTabName != null);
            var shellService = ServiceLocator.Default.ResolveType<IShellService>();
            shellService.Ribbon.AddRibbonGroupBox(this,RibbonTabName);
            this.Hide(null);
           
            _HasInitialized = true;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (Items.Count >= 0)
            {
                foreach (var item in Items)
                {
                    var element = item as UIElement;
                    if (element!=null&&element.Visibility==Visibility.Visible)
                    {
                        this.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }
            this.Visibility = Visibility.Collapsed;
        }

        public virtual void Show(object c)
        {
            this.Visibility = Visibility.Visible;
            this.DataContext = c;
        }
        public virtual void Hide(object c)
        {
            this.Visibility = Visibility.Collapsed;
            this.DataContext = c;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        virtual protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
