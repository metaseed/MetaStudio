

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace Metaseed.MetaShell.Controls
{
    using ViewModels;
    public class DocumentHeaderTemplateSelector : DataTemplateSelector
    {
        public DocumentHeaderTemplateSelector()
        {

        }


        public DataTemplate DocumentsHeaderTemplate
        {
            get;
            set;
        }

        public DataTemplate HostedProcessDocumentHeaderTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;
            if (itemAsLayoutContent == null) return base.SelectTemplate(item, container);
            itemAsLayoutContent.FloatingWidth =Application.Current.MainWindow.ActualWidth -160;
            if (itemAsLayoutContent.Content is HostedProcessDocumentViewModel)
                return HostedProcessDocumentHeaderTemplate; 
            if (itemAsLayoutContent.Content is DocumentBaseViewModel)
                return DocumentsHeaderTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}
