using System;
using Fluent;
using Metaseed.MetaShell.Services;

namespace Metaseed.MetaShell.Services
{
    public interface IRibbonService
    {
        IRibbonService AddRibbonContextualTabGroup(RibbonContextualTabGroup ribbonContextualTabGroup);
        IRibbonService AddRibbonTab(RibbonTabItem ribbonTab);
        IRibbonService RemoveRibbonTab(RibbonTabItem ribbonTab);
        IRibbonService AddRibbonGroupBox(RibbonGroupBox ribbonGroupBox, string ribbonTabItemName);
        IRibbonService AddRibbonControl(IRibbonControl ribbonControl, RibbonGroupBox ribbonGroupBox);
        IRibbonService AddRibbonControl(IRibbonControl ribbonControl, string ribbonTabItemName, string ribbonGroupBoxName);
        bool IsRibbonBackstageOpen { get; set; }

        event Action<RibbonTabItem> RibbonTabAdded;
        event Action<RibbonTabItem> RibbonTabRemoved;
    }
}
