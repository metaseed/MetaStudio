using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Catel.Logging;

namespace Metaseed.Modules.PropertyGrid
{
    using Xceed.Wpf.Toolkit.PropertyGrid;
    using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
    using Metaseed.Reflection;
    /// <summary>
    /// Note:
    /// [ExpandableObject()] is used to mark a property expendable
    /// </summary>
    public class MetaPropertyGrid : PropertyGrid
    {
        //show and updated property based on another property:http://wpftoolkit.codeplex.com/discussions/402950
        //Implement the INotifyPropertyChanged interface on your wrapper, and raise the PropertyChanged event with "Area" when your Height get modified, in your setter, just like you wrote.
        static ILog Log = LogManager.GetCurrentClassLogger();
        public MetaPropertyGrid()
        {
            PreparePropertyItem += MetaPropertyGrid_PreparePropertyItem;
        }
        //only show properties with BrowsableAttribute 
        string[] GetBrowsableProperties(object o)
        {
            var properties = o.GetType().GetProperties().Where(x => (Attribute.IsDefined(x, typeof(BrowsableAttribute), true)) && Attribute.GetCustomAttribute(x, typeof(BrowsableAttribute)).Equals(BrowsableAttribute.Yes)).Select(p => p.Name).ToArray();
            return properties;
        }
        protected override void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            string[] propertiesToShow = null;
            //Get the properties that are decorated with the Editable Attribute
            if (newValue != null)
            {
                propertiesToShow = GetBrowsableProperties(newValue);
                //clear the old definitions.
                PropertyDefinitions.Clear();
                //Add the properties to show.
                PropertyDefinitions.Add(new Xceed.Wpf.Toolkit.PropertyGrid.PropertyDefinition() { TargetProperties = propertiesToShow });
                //PropertyGrid.PropertyDefinitions.Add(new Xceed.Wpf.Toolkit.PropertyGrid.PropertyDefinition() { TargetProperties = new Type[]{typeof(bool)} });
            }
            base.OnSelectedObjectChanged(oldValue, newValue);
        }
        /// <summary>
        /// http://wpftoolkit.codeplex.com/discussions/443566
        /// </summary>
        /// <param name="pProperty"></param>
        /// <param name="pValue"></param>
        //static public void ChangePropertyReadOnly(PropertyItem pProperty, bool pValue)
        //{
        //    PropertyDescriptor descriptor = pProperty.PropertyDescriptor;
        //    ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
        //    FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly",
        //                                     System.Reflection.BindingFlags.NonPublic |
        //                                     System.Reflection.BindingFlags.Instance);
        //    fieldToChange.SetValue(attribute, pValue);
        //}
        static public void ChangePropertyReadOnly<TProperty>(System.Linq.Expressions.Expression<Func<TProperty>> propertyExpression,bool value)
        {
            //string propertyName = Catel.ExpressionHelper.GetPropertyName<TProperty>(propertyExpression, false);
            //object owner = Catel.ExpressionHelper.GetOwner<TProperty>(propertyExpression);
            //if (owner==null)
            //{
            //    Log.Warning("ChangePropertyReadOnly could not find owner of the property:" + propertyName);
            //    return;
            //}
            //// Create a PropertyDescriptor for "SpouseName" by calling the static GetProperties on TypeDescriptor.
            //PropertyDescriptor descriptor = TypeDescriptor.GetProperties(owner.GetType())[propertyName];
            //// Fetch the ReadOnlyAttribute from the descriptor. 
            //ReadOnlyAttribute attrib =descriptor.Attributes[typeof(ReadOnlyAttribute)] as  ReadOnlyAttribute;
            //if (attrib==null)
            //{
            //    Log.Warning("ChangePropertyReadOnly could not find ReadOnlyAttribute of the property:" + propertyName);
            //    return;
            //}
            //// Get the internal isReadOnly field from the ReadOnlyAttribute using reflection. 
            //FieldInfo isReadOnly = attrib.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
            //// Using Reflection, set the internal isReadOnly field. 
            //isReadOnly.SetValue(attrib, value);  
            AttributeExtensions.ChangePropertyAttributePrivateField(propertyExpression, typeof(ReadOnlyAttribute), "isReadOnly", value);
        }
        internal static Type GetListItemType(Type listType)
        {
            Type type = listType.GetInterfaces().FirstOrDefault((Type i) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
            if (!(type != null))
            {
                return null;
            }
            return type.GetGenericArguments()[0];
        }
        static public void ChangePropertyBrowsable<TProperty>(System.Linq.Expressions.Expression<Func<TProperty>> propertyExpression, bool value)
        {
            AttributeExtensions.ChangePropertyAttributePrivateField(propertyExpression, typeof(BrowsableAttribute), "browsable", value);
        }
        /// <summary>
        /// property editor resolve path:
        /// 1.Editor set with [Editor(typeof(Metaseed.Gauges.ColorRangeEditor), typeof(Metaseed.Gauges.ColorRangeEditor))]
        /// 2.Editor defined in resource dictionary that is add by propertyGridService.AddEditorDefinition(ColorRangeEditor.EditorDefinition);
        /// first property name then propertyType; see: EditorTemplateDefinition
        /// see ColorRangeEditor of gauge 
        /// 3.editor define in xceed PropertyGrid Assembly(DefaultEditor) CreateDefaultEditor
        /// 4.we could overide the editor in the methord below:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MetaPropertyGrid_PreparePropertyItem(object sender, PropertyItemEventArgs e)
        {
            var propertyItem = e.PropertyItem as PropertyItem;
            ITypeEditor typeEditor = null;
            if (ITypeEditors_PropertyName.ContainsKey(propertyItem.PropertyDescriptor.Name))
            {
                typeEditor = Activator.CreateInstance(ITypeEditors_PropertyName[propertyItem.PropertyDescriptor.Name]) as ITypeEditor;
            }
            else if (ITypeEditors_PropertyType.ContainsKey(propertyItem.PropertyType))
            {
                typeEditor = Activator.CreateInstance(ITypeEditors_PropertyType[propertyItem.PropertyType]) as ITypeEditor;
            }
            else
            {
                Type listItemType = GetListItemType(propertyItem.PropertyType);
                if (listItemType != null)
                {
                    if (!listItemType.IsPrimitive && !listItemType.Equals(typeof(string)))
                    {
                        typeEditor = new MetaCollectionEditor();//new CollectionEditor(); override the collection editor
                    }
                }
            }
            if (typeEditor!=null)
            {
                var frameworkElement = typeEditor.ResolveEditor(propertyItem);
                if (frameworkElement != null)
                {
                    //ContainerHelperBase.SetIsGenerated(frameworkElement, true);
                    propertyItem.Editor = frameworkElement;
                }
            }

            //if (propertyItem.PropertyType == typeof(SolidColorBrush))//propertyItem.PropertyDescriptor.Name
            //{
            //    ITypeEditor typeEditor = new SolidColorBrushEditor();
            //    var frameworkElement = typeEditor.ResolveEditor(propertyItem);
            //    if (frameworkElement != null)
            //    {
            //        //ContainerHelperBase.SetIsGenerated(frameworkElement, true);
            //        propertyItem.Editor = frameworkElement;
            //    }
            //}
            e.Handled = true;
        }
        Dictionary<string, Type> ITypeEditors_PropertyName = new Dictionary<string, Type>();
        Dictionary<Type, Type> ITypeEditors_PropertyType = new Dictionary<Type, Type>();
        public void SetPropertyEditor(Type propertyType, Type iTypeEditor)
        {
            Catel.Argument.IsOfType(() => iTypeEditor,typeof( ITypeEditor));
            if (ITypeEditors_PropertyType.ContainsKey(propertyType))
            {
                ITypeEditors_PropertyType.Remove(propertyType);
            }
            ITypeEditors_PropertyType.Add(propertyType, iTypeEditor);
        }
        public void SetPropertyEditor(string propertyName, Type iTypeEditor)
        {
            Catel.Argument.IsOfType(() => iTypeEditor, typeof(ITypeEditor));
            if (ITypeEditors_PropertyName.ContainsKey(propertyName))
            {
                ITypeEditors_PropertyName.Remove(propertyName);
            }
            ITypeEditors_PropertyName.Add(propertyName, iTypeEditor);
        }
        /// <summary>
        /// http://wpftoolkit.codeplex.com/wikipage?title=PropertyGrid
        ///  <xctk:EditorTemplateDefinition x:Key="ColorRangeEditor" TargetProperties="{x:Type dsb:ColorPoint}">
        ///  <xctk:EditorTemplateDefinition.EditingTemplate>
        ///      <DataTemplate>
        ///         <gauge:ColorRangeEditor ColorPoint="{Binding Value}" />
        ///     </DataTemplate>
        ///  </xctk:EditorTemplateDefinition.EditingTemplate>
        ///   </xctk:EditorTemplateDefinition>
        /// </summary>
        /// <param name="editorDefinition">Editor defined in resource dictionary</param>
        public void AddEditorTemplateDefinition(EditorTemplateDefinition editorDefinition)
        {
            if (!EditorDefinitions.Contains(editorDefinition))
            {
                EditorDefinitions.Add(editorDefinition);
            }
        }
    }
}
