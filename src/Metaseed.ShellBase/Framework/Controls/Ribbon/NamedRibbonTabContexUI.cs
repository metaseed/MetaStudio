using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Catel.IoC;
using Fluent;
using Metaseed.MetaShell.Services;
using Metaseed.Views;
using Metaseed.Windows.Threading;

namespace Metaseed.MetaShell.Controls
{
    public class NamedRibbonTabContexUI : IContextUI
    {
        private readonly string _ribbonTabName;
        private readonly bool _hideOtherTabsInSameGroup;
        private bool _HasInitialized;
        protected readonly IShellService ShellService;
        private RibbonTabItem _ribbonTabItem;
        protected Fluent.Ribbon Ribbon
        {
            get { return ((RibbonService)(ShellService.Ribbon)).Ribbon; }
        }
        public NamedRibbonTabContexUI(string ribbonTabName, bool hideOtherTabsInSameGroup = true)
        {
            ShellService = ServiceLocator.Default.ResolveType<IShellService>();
            _ribbonTabName = ribbonTabName;
            _hideOtherTabsInSameGroup = hideOtherTabsInSameGroup;
        }

        public bool HasInitialized
        {
            get { return _HasInitialized; }
        }

        public void Hide(object objectWithContext)
        {
            if (!_HasInitialized) Initialize();
            if (!_HasInitialized) return;
            if (_hideOtherTabsInSameGroup)
            {
                RibbonTabContextUIHelper.Hide(_ribbonTabItem);
            }
            else
            {
                _ribbonTabItem.IsSelected = false;
            }
            Ribbon.Refresh();
        }

        public void Initialize()
        {
            _ribbonTabItem = Ribbon.Tabs.FirstOrDefault(tab => tab.Name.Equals(_ribbonTabName));
            if (_ribbonTabItem == null)
                return;
            _HasInitialized = true;
        }

        public void Show(object objectWithContext)
        {
             Initialize();
            if (!_HasInitialized) return;
            if (_hideOtherTabsInSameGroup)
            {
                RibbonTabContextUIHelper.Show(_ribbonTabItem, Ribbon);
            }
            else
            {
                _ribbonTabItem.Visibility = Visibility.Visible;
                if(!Ribbon.IsCollapsed)
                    _ribbonTabItem.IsSelected = true;
            }
            Ribbon.Refresh();
        }
    }
}
