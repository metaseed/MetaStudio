using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Regions;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace Metaseed.MetaShell.Prism.Regions
{
    //https://avalondock.codeplex.com/discussions/390255
    //https://avalondock.codeplex.com/releases/view/92210
    // not using https://avalondock.codeplex.com/discussions/358632
    public class AvalonDockLayoutAnchorableRegionAdapter : RegionAdapterBase<LayoutAnchorable>
    {
        public AvalonDockLayoutAnchorableRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }

        protected override void Adapt(IRegion region, LayoutAnchorable regionTarget)
        {
            if (regionTarget == null)
                throw new ArgumentNullException("regionTarget");

            if (regionTarget.Content != null)
            {
                throw new InvalidOperationException();
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault();
            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems[0]);
                    }
                };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }

    }
}
