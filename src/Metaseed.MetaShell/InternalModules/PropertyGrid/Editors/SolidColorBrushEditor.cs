using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
namespace Metaseed.Modules.PropertyGrid
{
    using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
    using Xceed.Wpf.Toolkit.PropertyGrid;
    using Xceed.Wpf.Toolkit.Core.Converters;
    public class SolidColorBrushEditor:ColorEditor
    {
        static EditorTemplateDefinition _EditorDefinition;
        public static EditorTemplateDefinition EditorDefinition
        {
            get
            {
                if (_EditorDefinition != null)
                {
                    return _EditorDefinition;
                }
                _EditorDefinition = Application.Current.TryFindResource("SolidColorBrushEditor") as EditorTemplateDefinition;
                return _EditorDefinition;
            }
        }
        protected override IValueConverter CreateValueConverter()
        {
            return new SolidColorBrushToColorConverter();
        }
    }
}
