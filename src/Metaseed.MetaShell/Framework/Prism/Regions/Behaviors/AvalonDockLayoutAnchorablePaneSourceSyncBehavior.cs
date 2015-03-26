using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Catel;
using System.Collections;
using System.ComponentModel;
namespace Metaseed.MetaShell.Prism.Regions.Behaviors
{
    using Views;
    using Services;
    public class AvalonDockLayoutAnchorablePaneSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public static readonly string BehaviorKey = "AvalonDockLayoutAnchorablePaneSourceSyncBehavior";
        private bool _updatingActiveViewsInAnchorablePanelSelectedContentChanged;
        private LayoutAnchorablePane _AnchorablePane;

        public DependencyObject HostControl
        {
            get
            {
                return this._AnchorablePane;
            }

            set
            {
                this._AnchorablePane = value as LayoutAnchorablePane;
            }
        }

        /// <summary>
        /// Starts to monitor the <see cref="IRegion"/> to keep it in synch with the items of the <see cref="HostControl"/>.
        /// </summary>
        protected override void OnAttach()
        {

            this.SynchronizeItems();
            this._AnchorablePane.PropertyChanging += _AnchorablePane_PropertyChanging;
            this._AnchorablePane.PropertyChanged += _AnchorbalePane_PropertyChanged;
            this._AnchorablePane.Children.CollectionChanged += Children_CollectionChanged;
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
            this.Region.Views.CollectionChanged += this.Views_CollectionChanged;
        }
        bool _updatingAnchorablePaneSelectedContentFromFegionActiveViews = false;
        bool _updatingAnchorablePaneChildrenCollectionFromRegionViews = false;
        private void SynchronizeItems()
        {
            List<ToolViewBase> list = new List<ToolViewBase>();
            foreach (var current in this._AnchorablePane.Children)
            {
                if (current.Content is ToolViewModelBase)
                {
                    list.Add(current.Content as ToolViewBase);
                }
                else
                {
                    var c = current.Content;
                    current.Content = null;
                    var vm = new ToolViewModelBase(current.Title, this.Region.Name.GetToolPaneLocation());
                    var wraper = new ToolViewBase(vm) { Content = c };
                    list.Add(wraper);
                    current.Content = wraper;
                }
            }
            foreach (var current2 in base.Region.Views)
            {
                if (current2 is ToolViewBase)
                {
                    this._AnchorablePane.Children.Add(new LayoutAnchorable() { Content = current2 });
                }
                else
                {
                    throw new InvalidOperationException("Please make sure the content added to prism tool regions derived from the ToolViewBase class.");
                }
            }
            foreach (var current3 in list)
            {
                base.Region.Add(current3);
            }
        }
        bool _updatingViewsInLayoutAnchablePaneChildrenCollectionChanged = false;
        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_updatingAnchorablePaneSelectedContentFromFegionActiveViews || _updatingAnchorablePaneChildrenCollectionFromRegionViews)
            {
                return;
            }
            try
            {
                _updatingViewsInLayoutAnchablePaneChildrenCollectionChanged = true;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    //int startIndex = e.NewStartingIndex;
                    foreach (LayoutAnchorable newItem in e.NewItems)
                    {
                        var toolViewModel = newItem.Content as IToolView;
                        if (toolViewModel == null)
                        {
                            throw new InvalidCastException("AvalonDockLayoutAnchorablePaneSourceSyncBehavior:Children_CollectionChanged:newItem.Content as IToolViewModel");
                        }
                        Region.Add(toolViewModel);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (LayoutAnchorable oldItem in e.OldItems)
                    {
                        var item = oldItem.Content;
                        var _toolView = (from toolView in Region.Views where toolView == item select toolView).FirstOrDefault();
                        if (_toolView != null)
                        {
                            Region.Remove(_toolView);
                        }
                        else
                        {
                            throw new Exception("Error to remove content from tools panel!");
                        }
                    }
                }
            }
            finally
            {
                _updatingViewsInLayoutAnchablePaneChildrenCollectionChanged = false;
            }
        }


        void _AnchorablePane_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (_updatingAnchorablePaneSelectedContentFromFegionActiveViews || _updatingAnchorablePaneChildrenCollectionFromRegionViews)
            {
                return;
            }
            if (e.PropertyName == "SelectedContent")
            {
                try
                {
                    this._updatingActiveViewsInAnchorablePanelSelectedContentChanged = true;

                    if (_AnchorablePane == sender && _AnchorablePane.SelectedContentIndex != -1)
                    {
                        if (_AnchorablePane.Children.Count > _AnchorablePane.SelectedContentIndex)
                        {
                            object activeContent = _AnchorablePane.SelectedContent.Content;
                            if (activeContent != null && this.Region.Views.Contains(activeContent) && this.Region.ActiveViews.Contains(activeContent))
                            {
                                this.Region.Deactivate(activeContent);
                            }
                        }

                    }
                }
                finally
                {
                    this._updatingActiveViewsInAnchorablePanelSelectedContentChanged = false;
                }
            }

        }

        void _AnchorbalePane_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_updatingAnchorablePaneSelectedContentFromFegionActiveViews || _updatingAnchorablePaneChildrenCollectionFromRegionViews)
            {
                return;
            }
            if (e.PropertyName == "SelectedContent")
            {
                try
                {
                    this._updatingActiveViewsInAnchorablePanelSelectedContentChanged = true;

                    if ((_AnchorablePane == sender) && (_AnchorablePane.SelectedContent != null))
                    {
                        object activeContent = _AnchorablePane.SelectedContent.Content;
                        if (activeContent != null && this.Region.Views.Contains(activeContent) && !this.Region.ActiveViews.Contains(activeContent))
                        {
                            this.Region.Activate(activeContent);
                        }
                    }
                }
                finally
                {
                    this._updatingActiveViewsInAnchorablePanelSelectedContentChanged = false;
                }
            }
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._updatingActiveViewsInAnchorablePanelSelectedContentChanged || _updatingViewsInLayoutAnchablePaneChildrenCollectionChanged)
            {
                return;
            }
            try
            {
                _updatingAnchorablePaneChildrenCollectionFromRegionViews = true;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    int startIndex = e.NewStartingIndex;
                    foreach (object newItem in e.NewItems)
                    {
                        _AnchorablePane.Children.Insert(startIndex++, new LayoutAnchorable() { Content = newItem });
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var oldItem in e.OldItems)
                    {
                        var items = _AnchorablePane.Children;
                        var _anchorble = (from anchorble in items where anchorble.Content == oldItem select anchorble).FirstOrDefault();
                        if (_anchorble != null)
                        {
                            _AnchorablePane.Children.Remove(_anchorble);
                        }
                        else
                        {
                            throw new Exception("Error to remove content from tools panel!");
                        }
                    }
                }
            }
            finally
            {
                _updatingAnchorablePaneChildrenCollectionFromRegionViews = false;
            }
        }
       
        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._updatingActiveViewsInAnchorablePanelSelectedContentChanged || _updatingViewsInLayoutAnchablePaneChildrenCollectionChanged)
            {
                return;
            }
            try
            {
                _updatingAnchorablePaneSelectedContentFromFegionActiveViews = true;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (this._AnchorablePane.SelectedContent != null
                        && this._AnchorablePane.SelectedContent != e.NewItems[0]
                        && this.Region.ActiveViews.Contains(this._AnchorablePane.SelectedContent.Content))
                    {
                        this.Region.Deactivate(this._AnchorablePane.SelectedContent.Content);
                    }
                    var _layoutAnchable = (from layoutAnchable in _AnchorablePane.Children where layoutAnchable.Content == e.NewItems[0] select layoutAnchable).FirstOrDefault();
                    if (_layoutAnchable != null)
                    {
                        this._AnchorablePane.SelectedContentIndex = this._AnchorablePane.Children.IndexOf(_layoutAnchable);
                    }

                }
                else if (e.Action == NotifyCollectionChangedAction.Remove &&
                         e.OldItems.Contains(this._AnchorablePane.SelectedContent.Content))
                {
                    this._AnchorablePane.SelectedContentIndex = -1;
                }
            }
            finally
            {
                _updatingAnchorablePaneSelectedContentFromFegionActiveViews = false;
            }
        }

       


    }
}
