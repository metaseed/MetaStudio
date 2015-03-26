

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
    class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {

        }


        public DataTemplate DocumentsTemplate
        {
            get;
            set;
        }

        public DataTemplate ToolsTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is IToolViewModel)
                return ToolsTemplate;

            if (item is ILayoutContentViewModel)
                return DocumentsTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}
