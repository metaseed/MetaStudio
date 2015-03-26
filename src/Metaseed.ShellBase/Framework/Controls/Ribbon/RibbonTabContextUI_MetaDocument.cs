using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.ComponentModel;

using Fluent;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;

namespace Metaseed.MetaShell.Controls
{
    using Views;
    public class MetaDocumentRibbonTabContextUI : RibbonTabContextUI
    {
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void SetDataContext(object c)
        {
            base.SetDataContext(c);
        }
        public override void Show(object objectWithContext)
        {

            base.Show(objectWithContext);
        }
        public override void Hide(object objectWithContext)
        {
            base.Hide(objectWithContext);
   
        }

    }
}
