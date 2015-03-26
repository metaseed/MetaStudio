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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Collections.ObjectModel;
namespace Metaseed.Modules.PropertyGrid
{
    using Metaseed.Modules.FunctionBlock.Framework;
    /// <summary>
    /// Interaction logic for MetaDataSelector.xaml
    /// </summary>
    public partial class MetaDataSelector : UserControl, ITypeEditor
    {
        public MetaDataSelector()
        {
            InitializeComponent();
        }


    }
}
