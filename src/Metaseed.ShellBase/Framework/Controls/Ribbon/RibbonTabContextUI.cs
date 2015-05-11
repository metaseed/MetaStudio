using System;
using System.Collections.Specialized;
using Fluent;

using System.ComponentModel;
using System.Windows;
using System.Diagnostics;
using Catel.IoC;
using Metaseed.Windows.Threading;

namespace Metaseed.MetaShell.Controls
{
    using MetaShell.Services;
    using Metaseed.Views;
    public class RibbonTabContextUI : RibbonTabItem, IContextUI, INotifyPropertyChanged
    {
        //static readonly ILog Log = LogManager.GetCurrentClassLogger();
        static  protected Ribbon Ribbon;
        bool _HasInitialized = false;
        public bool HasInitialized { get { return _HasInitialized; } }
        public RibbonTabContextUI()
        {
            this.Visibility = Visibility.Collapsed;
            this.DataContext = null;
            this.Groups.CollectionChanged += Groups_CollectionChanged;
        }

        void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Groups.Count >= 0)
            {
                foreach (var item in Groups)
                {
                    var element = item as UIElement;
                    if (element != null && element.Visibility == Visibility.Visible)
                    {
                        this.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }
            this.Visibility = Visibility.Collapsed;
        }
        protected void AddToRibbon(RibbonContextualTabGroup group)
        {
            this.Group = group;
            var shellService = ServiceLocator.Default.ResolveType<IShellService>();
            if (string.IsNullOrEmpty(this.Name)) this.Name = this.GetType().Name;
            shellService.Ribbon.AddRibbonTab(this);
        }
        public virtual void Initialize()
        {
            //should call AddToRibbon in the derived class's ctor
            Debug.Assert(Group != null);
            Debug.Assert(Name != null);
            try
            {    //object o = TreeHelper.FindAncestor(di, (x) => { return (x is RibbonWindow); });
                //RibbonWindow rw =(RibbonWindow)o;
                if (Ribbon == null)
                {
                    var rw = Application.Current.MainWindow as RibbonWindow;
                    if (rw==null)
                    {
                        Ribbon = ServiceLocator.Default.ResolveType<Views.ShellRibbon>();
                    }
                    else
                    {
                        Ribbon = (Ribbon)rw.FindName("Part_Ribbon");
                    }
                    
                }
                //Group.Items.Add(this);
                //Ribbon.SelectedTabItem = this;
                this.Hide(null);
                _HasInitialized = true;
            }
            catch (Exception e)
            {
                throw new ApplicationException("could not find  Ribbon Control which is named Part_Ribbon in RibbonWindow, when Initiallize RibbonTabContexUI" + e.Message);
            }
        }

        public virtual void Close()
        {
            var shellService = ServiceLocator.Default.ResolveType<IShellService>();
            shellService.Ribbon.RemoveRibbonTab(this);
        }

        public virtual void SetDataContext(object c)
        {
            this.DataContext = c;
        }
        public virtual void Show(object c)
        {
            SetDataContext(c);
            if (Group.Visibility != Visibility.Visible)
            {
                //patch to RibbonContextualTabGroup static void OnVisibilityChanged
                //because  that methord will set the tabs visibility to the group's visibility.
                var visibilityBackup = new Visibility[Group.Items.Count];
                for (int i = 0; i < Group.Items.Count; i++)
                {
                    visibilityBackup[i] = Group.Items[i].Visibility;
                }
                //patch end
                Group.Visibility = Visibility.Visible;
                //patch code
                for (int i = 0; i < Group.Items.Count; i++)
                {
                    Group.Items[i].Visibility = visibilityBackup[i];
                }
            }
            this.Visibility = Visibility.Visible;
            this.IsSelected = true;
            Ribbon.Refresh();
        }
        public virtual void Hide(object c)
        {
            this.DataContext = null;
            this.Visibility = Visibility.Collapsed;
            foreach (var tabItem in Group.Items)
            {
                if (tabItem.Visibility == Visibility.Visible) return;
            }
            Group.Visibility = Visibility.Collapsed;

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
