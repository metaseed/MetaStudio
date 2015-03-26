using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using System.Collections;
using System;
namespace Metaseed.Modules.PropertyGrid.Views
{
    using Metaseed.MetaShell.Views;
    using ViewModels;
    /// <summary>
    /// Interaction logic for PropertyGridView.xaml
    /// </summary>
    public partial class PropertyGridView : ToolBaseView
    {
        public PropertyGridView(PropertyGridViewModel viewModel)
            : base(viewModel)
        {
            // The following line simply forces Visual Studio to copy the
            // WPF Toolkit DLL to the output folder.
            _propertyGrid = null;
            InitializeComponent();
            _propertyGrid.PreparePropertyItem += _propertyGrid_PreparePropertyItem;
            _propertyGrid.SelectedObjectChanged += _propertyGrid_SelectedObjectChanged;
            viewModel.PropertyGrid = _propertyGrid;
            if (viewModel.SelectedObject != null)
            {
                var temp = viewModel.SelectedObject;
                viewModel.SelectedObject = null;
                viewModel.SelectedObject = temp;
            }
        }

        void _propertyGrid_SelectedObjectChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if not the 
            //_propertyGrid.IsCategorized=!_propertyGrid.IsCategorized;
           // _propertyGrid.IsCategorized = !_propertyGrid.IsCategorized;
        }

        void _propertyGrid_PreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            // var propertyGrid = (sender as Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid);
            // Parent of top-level properties is the PropertyGrid itself.
            //bool isTopLevelProperty =
            //  (propertyItem.ParentElement is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid);
            //if (isTopLevelProperty && propertyItem.PropertyDescriptor.Name == "Friend")
            //{
            //    propertyItem.DisplayName = "Friend (renamed)";
            //}
            //-AttributeCollection attributes = TypeDescriptor.GetProperties(propertyItem.Instance)[propertyItem.PropertyDescriptor.Name].Attributes;
            //-var displayNameA=attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;


            //var propertyItem = e.PropertyItem as PropertyItem;
            //PropertyInfo property;
            //if (propertyItem.Instance is Metaseed.ComponentModel._)
            //{
            //     property = (propertyItem.Instance as Metaseed.ComponentModel._).Instance.GetType().GetProperties().Where(p => p.Name.Equals(propertyItem.PropertyDescriptor.Name)).First();
            //}
            //else
            //{
            //     property = propertyItem.Instance.GetType().GetProperties().Where(p => p.Name.Equals(propertyItem.PropertyDescriptor.Name)).First();
            //}
            
            //var displayNameAttribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).FirstOrDefault() as DisplayNameAttribute;
            //if (displayNameAttribute != null)
            //{
            //    propertyItem.DisplayName = displayNameAttribute.DisplayName;
            //}
            //var categoryAttribute = property.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
            //if (categoryAttribute != null)
            //{
            //    propertyItem.Category = categoryAttribute.Category;
            //}

            //var descriptionAttribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
            //if (descriptionAttribute != null)
            //{
            //    propertyItem.Description = descriptionAttribute.Description;
            //}
            
        }
    }


}
