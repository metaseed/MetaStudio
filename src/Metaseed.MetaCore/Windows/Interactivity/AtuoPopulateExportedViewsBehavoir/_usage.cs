using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Windows.Interactivity.AtuoPopulateExportedViewsBehavoir
{
    class usage
    {
        1.in bootloader register the default region behavior:

        //ConfigureDefaultRegionBehaviors

        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            factory.AddIfMissing("AutoPopulateExportedViewsBehavior", typeof(AutoPopulateExportedViewsBehavior));
            return factory;
        }

2. on every view set the attributes: 
    [ViewExport( RegionName = RegionNames.Toolbox)]
    [Export(ViewNames.GaugeDesignerStencils)]
    [ViewSortHint("02")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class GaugeDesignerStencils : DockableContent//, INavigationAware

    {...}

 

 

 
    }
}
