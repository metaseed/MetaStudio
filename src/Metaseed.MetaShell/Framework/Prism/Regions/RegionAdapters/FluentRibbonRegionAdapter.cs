using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Regions;
using Fluent;
using System.Collections.Specialized;

using System.ComponentModel.Composition;
namespace Metaseed.MetaShell.Prism.Regions
{
    //http://compositewpf.codeplex.com/Thread/View.aspx?ThreadId=235004
    //http://blogsprajeesh.blogspot.com/2009/09/prism-creating-custom-region-adapter.html
    [Export(typeof(FluentRibbonRegionAdapter))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FluentRibbonRegionAdapter : RegionAdapterBase<Ribbon>
    {
        [ImportingConstructor]
        public FluentRibbonRegionAdapter(IRegionBehaviorFactory regionBehaviourFactory) : base(regionBehaviourFactory) { }

        //Maintain the ribbon instance
        private Ribbon _Ribbon;
        private IRegion _Region;
        private void SynchronizeItems()
        {
            List<object> list = new List<object>();
            foreach (object current in this._Ribbon.Tabs)
            {
                list.Add(current);
            }
            foreach (RibbonTabItem current2 in _Region.Views)
            {
                this._Ribbon.Tabs.Add(current2);
            }
            foreach (object current3 in list)
            {
                _Region.Add(current3);
            }

        }
        protected override void Adapt(IRegion region, Ribbon ribbon)
        {
            IRegionManager regionManager = (IRegionManager)Catel.IoC.ServiceLocator.Default.GetService(typeof(IRegionManager));
            regionManager.Regions.Add(region);
            _Ribbon = ribbon;
            _Region=region;
            SynchronizeItems();
            // ribbon.Tabs.Clear();
            //Implementing the collectionChanged handler
            region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
            region.Views.CollectionChanged += Views_CollectionChanged;
            _Ribbon.SelectedTabChanged += _Ribbon_SelectedTabChanged;
            
        }

        private bool _updatingActiveViewsInManagerActiveContentChanged;

        void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startIndex = e.NewStartingIndex;
                foreach (RibbonTabItem newItem in e.NewItems)
                {
                    _Ribbon.Tabs.Insert(startIndex++, newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (RibbonTabItem oldItem in e.OldItems)
                {
                    _Ribbon.Tabs.Remove(oldItem);
                }
            }
        }
        void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._updatingActiveViewsInManagerActiveContentChanged)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this._Ribbon.SelectedTabItem != null
                    && this._Ribbon.SelectedTabItem != e.NewItems[0]
                    && this._Region.ActiveViews.Contains(this._Ribbon.SelectedTabItem))
                {
                    this._Region.Deactivate(this._Ribbon.SelectedTabItem);
                }

                this._Ribbon.SelectedTabItem = (RibbonTabItem)e.NewItems[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove &&
                     e.OldItems.Contains(this._Ribbon.SelectedTabItem))
            {
                this._Ribbon.SelectedTabIndex = -1;
            }
        }
        void _Ribbon_SelectedTabChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                this._updatingActiveViewsInManagerActiveContentChanged = true;

                if (_Ribbon == sender)
                {
                    object activeContent = _Ribbon.SelectedTabItem;
                    foreach (object view in e.RemovedItems)
                    {
                        if (this._Region.Views.Contains(view) && this._Region.ActiveViews.Contains(view))
                            this._Region.Deactivate(view);
                    }
                    foreach (object view in e.AddedItems)
                    {
                        if (this._Region.Views.Contains(view) && !this._Region.ActiveViews.Contains(view))
                            this._Region.Activate(view);
                    }
                }
            }
            finally
            {
                this._updatingActiveViewsInManagerActiveContentChanged = false;
            }
        }
        protected override IRegion CreateRegion()
        {
            //This region keeps all the views in it as active.
            //Deactivation of views is not allowed.
            //This is the region used for ItemsControl controls.
            return new Region();
        }

    }
}
//Once the adapter is ready you can register it with the RegionAdapterMappings
//var __Mappings = base.ConfigureRegionAdapterMappings();
//__Mappings.RegisterMapping(typeof(Microsoft.Windows.Controls.Ribbon.Ribbon), this.Container.Resolve<OfficeRibbonRegionAdapter>());
//return __Mappings;

//Now we can create RibbonTabs in individual modules
//RibbonTab __FolderTab = new RibbonTab();
//__FolderTab.Label = "Folder";
//RibbonGroup __FolderGroup = new RibbonGroup();
//__FolderGroup.GroupSizeDefinitions = FindResource("FolderGroup") as Collection<RibbonGroupSizeDefinition>;
//RibbonButton __DocButton = CreateRibbonButton("DocCommand");
//RibbonButton __MusicButton = CreateRibbonButton("MusicCommand");
//RibbonButton __PictureButton = CreateRibbonButton("PictureCommand");
//RibbonButton __VideosButton = CreateRibbonButton("VideosCommand");
//__FolderGroup.Controls.Add(__DocButton);
//__FolderGroup.Controls.Add(__MusicButton);
//__FolderGroup.Controls.Add(__PictureButton);
//__FolderGroup.Controls.Add(__VideosButton);
//__FolderTab.Groups.Add(__FolderGroup);
//return __FolderTab;

//Once the ribbon tabs are ready you can add it to the Region using ViewDiscoveryUIComposition method or using the Add method like
//___RegionManager.RegisterViewWithRegion("ShellRibbon", () => ___Container.Resolve<RibbonView>().FolderRibbonTab);
//Or
//___RegionManager.Regions["ShellRibbon"].Add(___RibbonView.OfficeRibbonTab);