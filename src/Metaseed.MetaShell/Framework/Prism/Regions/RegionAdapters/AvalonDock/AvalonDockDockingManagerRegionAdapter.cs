using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock;
using Microsoft.Practices.Prism.Regions;

namespace Metaseed.MetaShell.Prism.Regions
{
    using Behaviors;
    //https://avalondock.codeplex.com/discussions/390255
    //https://avalondock.codeplex.com/releases/view/92210
    //not using https://avalondock.codeplex.com/discussions/358632
    public class AvalonDockDockingManagerRegionAdapter : RegionAdapterBase<DockingManager>
    {
        public AvalonDockDockingManagerRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }

        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            IRegionManager regionManager = (IRegionManager)Catel.IoC.ServiceLocator.Default.GetService(typeof(IRegionManager));
            regionManager.Regions.Add(region);
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }

        protected override void AttachBehaviors(IRegion region, DockingManager regionTarget)
        {
            if (region == null)
                throw new System.ArgumentNullException("region");

            // Add the behavior that syncs the items source items with the rest of the items
            region.Behaviors.Add(AvalonDockDockingManagerDocumentsSourceSyncBehavior.BehaviorKey, new AvalonDockDockingManagerDocumentsSourceSyncBehavior()
            {
                HostControl = regionTarget
            });

            base.AttachBehaviors(region, regionTarget);
        }
    }
}
