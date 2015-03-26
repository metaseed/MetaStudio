using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Catel.Logging;
using Catel.IoC;
using Catel;
using Catel.MVVM;
using System.Windows.Controls;
using System.Windows.Data;
using Catel.Windows;
using Metaseed.MVVM.ViewModel;

namespace Metaseed.MVVM.View
{
    public class View
    {
        static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        public static object GetModel(DependencyObject obj)
        {
            return (object)obj.GetValue(ModelProperty);
        }

        public static void SetModel(DependencyObject obj, object value)
        {
            obj.SetValue(ModelProperty, value);
        }

        /// <summary>
        /// A dependency property for attaching a View(got from ViewModel by MVVM Name Convension) to the Content Control.
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached("Model", typeof(object), typeof(View), new PropertyMetadata(null, new PropertyChangedCallback(View.OnModelChanged)));
        private static void OnModelChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue == args.NewValue)
            {
                return;
            }
            if (args.NewValue == null)
                return;//throw new Exception("The View.Model Dependency Property Could Only Be Set On The ContentControl");
            var viewModel = args.NewValue;
            var viewLocator = ServiceLocator.Default.ResolveType<IViewLocator>();
            var viewType = viewLocator.ResolveView(viewModel.GetType());
            FrameworkElement view = null;
            if (viewType == null)
            {
                Logger.Warning("Could not find ViewType of ViewModel-{0}, will use a Text Control", viewModel.GetType().Name);
                view = new TextBlock
                {
                    Text = string.Format("Cannot find view for view model:{0}.", viewModel.GetType().Name),
                    DataContext = viewModel
                };
            }
            else
            {
                view = ViewHelper.ConstructViewWithViewModel(viewType, viewModel);
            }
            var contentControl = targetLocation as ContentControl;
            if (contentControl != null)
            {
                contentControl.Content = view;
            }
            else
            {
                View.SetContentProperty(targetLocation, view);
            }
            var viewM = viewModel as ICreatViewAsContent;
            if (viewM != null)
            {
                viewM.SetViewAsContent(contentControl, view, viewM);
                //var layoutDocControl = view.FindVisualAncestor(view, (v) => v is LayoutDocumentControl);
            }
            
        }
        private static void SetContentProperty(object targetLocation, object view)
        {
            var fe = view as FrameworkElement;
            if (fe != null && fe.Parent != null)
            {
                View.SetContentPropertyCore(fe.Parent, null);
            }
            View.SetContentPropertyCore(targetLocation, view);
        }
        private static void SetContentPropertyCore(object targetLocation, object view)
        {
            try
            {
                Type type = targetLocation.GetType();
                ContentPropertyAttribute contentProperty = Attribute.GetCustomAttributes(type, true).OfType<ContentPropertyAttribute>().FirstOrDefault<ContentPropertyAttribute>() ?? View.DefaultContentProperty;
                type.GetProperty(contentProperty.Name).SetValue(targetLocation, view, null);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        private static readonly ContentPropertyAttribute DefaultContentProperty = new ContentPropertyAttribute("Content");
    }
}
