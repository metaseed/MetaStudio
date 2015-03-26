using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
namespace Metaseed.MetaShell.Prism.Regions
{
    using Behaviors;
    public class AvalonDockLayoutAnchorablePaneRegionAdapter : RegionAdapterBase<LayoutAnchorablePane>
    {
        public AvalonDockLayoutAnchorablePaneRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }
        protected override void Adapt(IRegion region, LayoutAnchorablePane regionTarget)
        {
            IRegionManager regionManager = (IRegionManager)Catel.IoC.ServiceLocator.Default.GetService(typeof(IRegionManager));
            regionManager.Regions.Add(region);
        }
        protected override IRegion CreateRegion()
        {
            return new Region();
        }

        protected override void AttachBehaviors(IRegion region, LayoutAnchorablePane regionTarget)
        {
            if (region == null)
                throw new System.ArgumentNullException("region");

            // Add the behavior that syncs the items source items with the rest of the items
            region.Behaviors.Add(AvalonDockLayoutAnchorablePaneSourceSyncBehavior.BehaviorKey, new AvalonDockLayoutAnchorablePaneSourceSyncBehavior()
            {
                HostControl = regionTarget//,DockingManager=ServiceLocator.Current.GetInstance<DockingManager>()
            });

            base.AttachBehaviors(region, regionTarget);
        }
    }
}
