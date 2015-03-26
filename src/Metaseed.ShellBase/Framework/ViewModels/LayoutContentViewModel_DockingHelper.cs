using System;
using System.Windows.Controls;
using System.Windows.Data;
using Metaseed.Windows.Media;
using Metaseed.MVVM.ViewModel;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Metaseed.MetaShell.ViewModels
{
    public partial class LayoutContentViewModel: ICreatViewAsContent
    {
        //private LayoutItem _layoutItem;
        //public LayoutItem LayoutItem
        //{
        //    get { return _layoutItem; }
        //    set
        //    {
        //        if (value == null || value.Equals(_layoutItem)) return;
        //        _layoutItem = value;
        //        RaisePropertyChanged(() => LayoutItem);
        //    }
        //}

        private LayoutContent _layoutContent;
        /// <summary>
        /// be  LayoutAnchorable or LayoutDocument
        /// </summary>
        public LayoutContent LayoutContent
        {
            get { return _layoutContent; }
            set
            {
                if (value != null) IsFloating = value.IsFloating;
                if (value == null || value.Equals(_layoutContent)) return;
                _layoutContent = value;
                RaisePropertyChanged(() => LayoutContent);
            }
        }

        bool _isFloating;

        public bool IsFloating
        {
            get { return _isFloating; }
            set
            {
                if (_isFloating == value) return;
                _isFloating = value;
                OnIsFloatingChanged(value);
                RaisePropertyChanged("IsFloating");
            }
        }

        virtual protected void OnIsFloatingChanged(bool isFloating)
        {
            
        }

        public void SetViewAsContent(ContentControl contentControl, FrameworkElement view, ICreatViewAsContent viewModel)
        {
            var contentViewModel = viewModel as LayoutContentViewModel;
            if (contentViewModel == null) return;
            var docViewModel =  contentViewModel as DocumentBaseViewModel;
            if (docViewModel != null)
            {
                var docCtrl = contentControl.FindVisualAncestor<LayoutDocumentControl>(8);
                if (docCtrl != null)
                {
                    //docCtrl.SetBinding(LayoutDocumentControl.ModelProperty,new Binding("Model") { Source = contentViewModel,Mode = BindingMode.OneWayToSource});
                    LayoutContent = docCtrl.Model;
                }
            }
            else
            {
                var anchorCtrl = contentControl.FindVisualAncestor<LayoutAnchorableControl>(8);
                if (anchorCtrl != null)
                {
                    LayoutContent = anchorCtrl.Model;
                    //anchorCtrl.SetBinding(LayoutAnchorableControl.ModelProperty, new Binding("Model") { Source = contentViewModel, Mode = BindingMode.OneWayToSource });
                }
            }
            
        }
    }
}
