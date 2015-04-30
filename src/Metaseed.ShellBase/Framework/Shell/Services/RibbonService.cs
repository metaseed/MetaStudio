using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Fluent;
using Metaseed.MetaShell.Views;

namespace Metaseed.MetaShell.Services{
    public class RibbonService:IRibbonService
    {
        static readonly ILog Log = LogManager.GetCurrentClassLogger();
        Ribbon _ribbon;
        Ribbon Ribbon
        {
            get
            {
                if (_ribbon == null)
                {
                    _ribbon = ServiceLocator.Default.ResolveType<ShellRibbon>();
                    _ribbon.Tabs.CollectionChanged += Tabs_CollectionChanged;
                    ScreenTip.HelpPressed += OnScreenTipHelpPressed;
                }
                return _ribbon;
            }
        }

        void Tabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action==NotifyCollectionChangedAction.Add)
            {
                foreach (RibbonTabItem tab in e.NewItems)
                {
                    if (RibbonTabAdded!=null)
                    this.RibbonTabAdded(tab);
                }
            }else if (e.Action == NotifyCollectionChangedAction.Remove )
            {
                foreach (RibbonTabItem tab in e.OldItems)
                {
                    if (RibbonTabRemoved!=null)
                    this.RibbonTabRemoved(tab);
                }
            }
        }

        public event Action<RibbonTabItem> RibbonTabAdded;
        public event Action<RibbonTabItem> RibbonTabRemoved;
        /// <summary>
        /// Handles F1 pressed on ScreenTip with help capability
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        static void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            // Show help according the given help topic
            // (here just show help topic as string)
            MessageBox.Show(e.HelpTopic.ToString());
        }
        public IRibbonService AddRibbonContextualTabGroup(RibbonContextualTabGroup ribbonContextualTabGroup)
        {
            if (!ribbonContextualTabGroup.Dispatcher.CheckAccess())
            {
                ribbonContextualTabGroup.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Func<RibbonContextualTabGroup, IRibbonService>(AddRibbonContextualTabGroup), ribbonContextualTabGroup);
                return this;
            }
            Ribbon.ContextualGroups.Add(ribbonContextualTabGroup);
            return this;
        }
        public IRibbonService AddRibbonTab(RibbonTabItem ribbonTab)
        {
            if (!ribbonTab.Dispatcher.CheckAccess())
            {
                ribbonTab.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Func<RibbonTabItem, IRibbonService>(AddRibbonTab), ribbonTab);
                return this;
            }
            Argument.IsNotNull("ribbonTab.Name", ribbonTab.Name);
            for (int i = Ribbon.Tabs.Count - 1; i >= 0; i--)
            {
                if (Ribbon.Tabs[i].Group != null && Ribbon.Tabs[i].Group.Equals(ribbonTab.Group))
                {
                    Ribbon.Tabs.Insert(i + 1, ribbonTab);
                    return this;
                }
            }
            Ribbon.Tabs.Add(ribbonTab);
            return this;
        }

        public IRibbonService RemoveRibbonTab(RibbonTabItem ribbonTab)
        {
            if (!ribbonTab.Dispatcher.CheckAccess())
            {
                ribbonTab.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Func<RibbonTabItem, IRibbonService>(RemoveRibbonTab), ribbonTab);
                return this;
            }
            if (ribbonTab.Group != null && ribbonTab.Group.Items.Count == 1)
            {
                Ribbon.Tabs.Remove(ribbonTab);
                Ribbon.ContextualGroups.Remove(ribbonTab.Group);
            }
            else
            {
                Ribbon.Tabs.Remove(ribbonTab);
            }
            return this;
        }

        public IRibbonService AddRibbonControl(IRibbonControl ribbonControl, RibbonGroupBox ribbonGroupBox)
        {
            Argument.IsNotNull("ribbonControl", ribbonControl);
            Argument.IsNotNull("ribbonGroupBox", ribbonGroupBox);
            Argument.IsNotNull("ribbonGroupBox.Name", ribbonGroupBox.Name);
            ribbonGroupBox.Items.Add(ribbonControl);
            return this;
        }
        public IRibbonService AddRibbonGroupBox(RibbonGroupBox ribbonGroupBox, string ribbonTabItemName)
        {
            Argument.IsNotNull("ribbonTabItemName", ribbonTabItemName);
            Argument.IsNotNull("ribbonGroupBox.Name", ribbonGroupBox.Name);
            RibbonTabItem ribbonTabItem = Ribbon.Tabs.FirstOrDefault(tab => tab.Name.Equals(ribbonTabItemName));
            if (ribbonTabItem == null)
            {
                string error = string.Format("could not find the RibbonTabItem of name:{0}--AddRibbonGroupBox", ribbonTabItemName);
                Log.Error(error);
                return this;
            }
            ribbonTabItem.Groups.Add(ribbonGroupBox);
            return this;
        }
        public IRibbonService AddRibbonControl(IRibbonControl ribbonControl, string ribbonTabItemName, string ribbonGroupBoxName)
        {
            Argument.IsNotNull("ribbonTabItemName", ribbonTabItemName);
            Argument.IsNotNull("ribbonGroupBoxName", ribbonGroupBoxName);
            RibbonTabItem ribbonTabItem = Ribbon.Tabs.FirstOrDefault(tab => tab.Name.Equals(ribbonTabItemName));
            if (ribbonTabItem == null)
            {
                string error = string.Format("could not find the RibbonTabItem of name:{0}--AddRibbonControl", ribbonTabItemName);
                Log.Error(error);
                return this;
            }

            RibbonGroupBox ribbonGroupBox = null;
            foreach (var groupBox in ribbonTabItem.Groups)
            {
                if (groupBox.Name.Equals(ribbonGroupBoxName))
                {
                    ribbonGroupBox = groupBox;
                    break;
                }
            }
            if (ribbonGroupBox == null)
            {
                string error = string.Format("could not find the RibbonGroupBox of name:{0}--AddRibbonControl", ribbonGroupBoxName);
                Log.Error(error);
                return this;
            }
            return AddRibbonControl(ribbonControl, ribbonGroupBox);

        }

        public bool IsRibbonBackstageOpen
        {
            get
            {
                var backstage = Ribbon.Menu as Backstage;
                if (backstage != null)
                    return backstage.IsOpen;
                return false;
            }
            set
            {
                var backstage = Ribbon.Menu as Backstage;
                if (backstage != null)
                    backstage.IsOpen = value;
            }
        }

    }
}
