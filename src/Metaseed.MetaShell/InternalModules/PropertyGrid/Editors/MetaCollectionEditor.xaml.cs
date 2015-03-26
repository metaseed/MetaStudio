/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Metaseed.Modules.PropertyGrid//Xceed.Wpf.Toolkit.PropertyGrid.Editors
{
  /// <summary>
  /// Interaction logic for CollectionEditor.xaml
  /// </summary>
    public partial class MetaCollectionEditor : UserControl, ITypeEditor
  {
    PropertyItem _item;

    public MetaCollectionEditor()
    {
      InitializeComponent();
    }
    internal static T GetAttribute<T>(PropertyDescriptor property) where T : Attribute
    {
        return property.Attributes.OfType<T>().FirstOrDefault();
    }
    internal object ComputeNewItemTypesForItem(object item)
    {
        PropertyDescriptor pd = item as PropertyDescriptor;
        var attribute = GetAttribute<NewItemTypesAttribute>(pd);

        return (attribute != null)
                ? attribute.Types
                : null;
    }
    private void Button_Click( object sender, RoutedEventArgs e )
    {
        MetaCollectionControlDialog editor = new MetaCollectionControlDialog(_item.PropertyType, (IList<Type>)ComputeNewItemTypesForItem(_item.PropertyDescriptor));
        Binding binding = new Binding("Value");
        editor.IsReadOnly = _item.IsReadOnly;
        binding.Source = _item;
        binding.Mode = _item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
        BindingOperations.SetBinding(editor, MetaCollectionControlDialog.ItemsSourceProperty, binding);
        editor.ShowDialog();
    }


    public FrameworkElement ResolveEditor( PropertyItem propertyItem )
    {
      _item = propertyItem;
      return this;
    }
  }
}
