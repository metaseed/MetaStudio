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
using Microsoft.Practices.ServiceLocation;
namespace Metaseed.MetaShell.Prism.Regions.Behaviors
{
    using Views;
    using Infrastructure;
    public class AvalonDockDockingManagerDocumentsSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public static readonly string BehaviorKey = "DockingManagerDocumentsSourceSyncBehavior";
       
        private DockingManager _dockingManager;

        public DependencyObject HostControl
        {
            get
            {
                return this._dockingManager;
            }

            set
            {
                this._dockingManager = value as DockingManager;
            }
        }

        ObservableCollection<IDocumentView> _documents = new ObservableCollection<IDocumentView>();

        public ObservableCollection<IDocumentView> Documents
        {
            get
            {
                return _documents;
            }

        }

        /// <summary>
        /// Starts to monitor the <see cref="IRegion"/> to keep it in synch with the items of the <see cref="HostControl"/>.
        /// </summary>
        protected override void OnAttach()
        {
            bool itemsSourceIsSet = this._dockingManager.DocumentsSource != null;


            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException();
            }

            this.SynchronizeItems();

            this._dockingManager.ActiveContentChanged += this.ManagerActiveContentChanged;
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
            this.Region.Views.CollectionChanged += this.Views_CollectionChanged;
            Documents.CollectionChanged += ManagerDocuments_CollectionChanged;  
            
        }
        private void SynchronizeItems()
        {
            LayoutDocumentPane layoutDocumentPane = ServiceLocator.Current.GetInstance<LayoutDocumentPane>();

            foreach (IDocumentView view in this.Region.Views)
            {
                _documents.Add(view);
            }

            foreach (var current in layoutDocumentPane.Children)
            {
                var content = current.Content;
                if (content is DocumentViewBase)
                {
                    _documents.Add(content as IDocumentView);
                }
                else
                {
                    var viewModel = new DocumentViewModelBase(current.Title)
                    {

                    };
                    var view = new DocumentViewBase(viewModel) { Content = content };
                    _documents.Add(view);
                }

            }
            layoutDocumentPane.Children.Clear();
            BindingOperations.SetBinding(
                _dockingManager,
                DockingManager.DocumentsSourceProperty,
                new Binding("Documents") { Source = this });
            foreach (var item in _documents)
            {
                this.Region.Add(item);
            }
            if (Documents.Count > 0)
            {
                this.Region.Activate(Documents[0]);
            }

        }
        bool _updatingViewsFromManagerDocumentsCollectionChanged = false;
        bool _updatingActiveViewsFromManagerActiveContentChanged = false;
        bool _updatingManagerChildrenDocumentCollectionFromRegionViewsChanged = false;
        bool _updatingManagerActiveContentFromRegionActiveViewChanged = false;
        private void ManagerActiveContentChanged(object sender, EventArgs e)
        {
            if (_updatingManagerActiveContentFromRegionActiveViewChanged||_updatingManagerChildrenDocumentCollectionFromRegionViewsChanged)
            {
                return;
            }
            try
            {
                this._updatingActiveViewsFromManagerActiveContentChanged = true;

                if (_dockingManager == sender)
                {
                    object activeContent = _dockingManager.ActiveContent;
                    foreach (var item in this.Region.ActiveViews.Where(it => it != activeContent))
                    {
                        this.Region.Deactivate(item);
                    }


                    if (this.Region.Views.Contains(activeContent) && !this.Region.ActiveViews.Contains(activeContent))
                    {
                        this.Region.Activate(activeContent);
                    }
                }
            }
            finally
            {
                this._updatingActiveViewsFromManagerActiveContentChanged = false;
            }
        }
        
        void ManagerDocuments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_updatingManagerActiveContentFromRegionActiveViewChanged || _updatingManagerChildrenDocumentCollectionFromRegionViewsChanged)
            {
                return;
            }
            try
            {
            _updatingViewsFromManagerDocumentsCollectionChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //int startIndex = e.NewStartingIndex;
                foreach (LayoutDocument newItem in e.NewItems)
                {
                    var documentViewModel = newItem.Content as IDocumentView;
                    if (documentViewModel == null)
                    {
                        throw new InvalidCastException("AvalonDockDockingManagerDocumentsSourceSyncBehavior:Children_CollectionChanged:newItem.Content as IDocumentView");
                    }
                    Region.Add(documentViewModel);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (LayoutDocument oldItem in e.OldItems)
                {
                    var item = oldItem.Content;
                    var _documentView = (from documentView in Region.Views where documentView == item select documentView).FirstOrDefault();
                    if (_documentView != null)
                    {
                        Region.Remove(_documentView);
                    }
                    else
                    {
                        throw new Exception("Error to remove content from document panel!");
                    }
                }
            }
            }
            finally
            {

                _updatingViewsFromManagerDocumentsCollectionChanged=false;
            }
        }
       
        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_updatingViewsFromManagerDocumentsCollectionChanged || _updatingActiveViewsFromManagerActiveContentChanged)
            {
                return;
            }
            try
            {
                _updatingManagerChildrenDocumentCollectionFromRegionViewsChanged = true;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    int startIndex = e.NewStartingIndex;
                    foreach (object newItem in e.NewItems)
                    {
                        _documents.Insert(startIndex++, newItem as IDocumentView);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (object oldItem in e.OldItems)
                    {
                        _documents.Remove(oldItem as IDocumentView);
                    }
                }
            }
            finally
            {
                _updatingManagerChildrenDocumentCollectionFromRegionViewsChanged = false;
            }
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._updatingActiveViewsFromManagerActiveContentChanged || _updatingViewsFromManagerDocumentsCollectionChanged)
            {
                return;
            }
            try
            {
                _updatingManagerActiveContentFromRegionActiveViewChanged = true;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (this._dockingManager.ActiveContent != null
                        && this._dockingManager.ActiveContent != e.NewItems[0]
                        && this.Region.ActiveViews.Contains(this._dockingManager.ActiveContent))
                    {
                        this.Region.Deactivate(this._dockingManager.ActiveContent);
                    }

                    this._dockingManager.ActiveContent = e.NewItems[0];
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove &&
                         e.OldItems.Contains(this._dockingManager.ActiveContent))
                {
                    this._dockingManager.ActiveContent = null;
                }
            }
            finally
            {
                _updatingManagerActiveContentFromRegionActiveViewChanged = false;
            }
        }

       

    }
}
