

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace Metaseed.MetaShell.Controls
{
    using ViewModels;
    public class DocumentTitleTemplateSelector : DataTemplateSelector
    {
        public DocumentTitleTemplateSelector()
        {

        }


        public DataTemplate DocumentsTitleTemplate
        {
            get;
            set;
        }

        public DataTemplate HostedProcessDocumentTitleTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;
            if (itemAsLayoutContent == null) return base.SelectTemplate(item, container);
            if (itemAsLayoutContent.Content is HostedProcessDocumentViewModel)
                return HostedProcessDocumentTitleTemplate;
            if (itemAsLayoutContent.Content is DocumentBaseViewModel)
                return DocumentsTitleTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}
