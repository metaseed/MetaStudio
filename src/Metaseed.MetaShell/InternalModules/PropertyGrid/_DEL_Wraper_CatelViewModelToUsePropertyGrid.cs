using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using System.Collections;
namespace Metaseed.ComponentModel
{
    //Note:
    //the new nightly build Catel has solved this problem!! please upgrade it!
    //https://www.nuget.org/packages/Catel.Core/3.6.1310102241-beta
    //and this is the problem solving progress board:
    //https://catelproject.atlassian.net/browse/CTL-186
    //before this night build i have to ceat a wrapper class for viewmodel and using it to display and edit properties of viewmodelbase derived class.
    //so there is no reason for this class to exist

    //https://wpftoolkit.codeplex.com/discussions/435532
    //https://catelproject.atlassian.net/browse/CTL-186
    public class PropertyDescriptor_ForCatelViewModelToUsePropertyGrid : PropertyDescriptor
    {
        PropertyInfo _propertyInfo;
        object _Instance;
        public PropertyDescriptor_ForCatelViewModelToUsePropertyGrid(object instance, PropertyInfo property, Attribute[] attributeArray)
            : base(property.Name, attributeArray)
        {
            _Instance = instance;
            _propertyInfo = property;
        }
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public override Type ComponentType
        {
            get
            {
                return this._Instance.GetType();
            }
        }
        public override Type PropertyType
        {
            get
            {
                return this._propertyInfo.PropertyType;
            }
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override object GetValue(object component)
        {

            return this._propertyInfo.GetValue(this._Instance, null);
        }
        public override void ResetValue(object component)
        {
            throw new NotSupportedException("Resetting values is not supported");
        }
        public override void SetValue(object component, object value)
        {
            this._propertyInfo.SetValue(this._Instance, value, null);
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
    /// <summary>
    /// Wraper_CatelViewModelToUsePropertyGrid
    /// 
    /// usage:
    /// if (value is Catel.MVVM.ViewModelBase)
    //{
    //    PropertyGrid.SelectedObject = new _(value, propertiesToShow);
    //}
    /// </summary>
    public class _ : ICustomTypeDescriptor
    {
       public object Instance;
       string[] _PropertiesToShow;
       public _(object anInstance, string[] propertiesToShow)
        {
            Instance = anInstance;
            _PropertiesToShow = propertiesToShow;
        }

        // Returns the properties you want to edit for your SomeClass instance
        public PropertyDescriptorCollection GetProperties()
        {
            ArrayList properties = new ArrayList();

            PropertyInfo[] objectProperties = Instance.GetType().GetProperties();
            foreach (PropertyInfo property in objectProperties)
            {
                if (!_PropertiesToShow.Contains(property.Name))
                {
                    continue;
                }
                ArrayList attributeList = new ArrayList();
                attributeList.AddRange(property.GetCustomAttributes(false));
                // Add in your property descriptor
                Attribute[] attributeArray = (Attribute[])attributeList.ToArray(typeof(Attribute));
                properties.Add(new PropertyDescriptor_ForCatelViewModelToUsePropertyGrid(Instance, property, attributeArray));
            }

            // Finally, return your property descriptor collection
            PropertyDescriptor[] propertyArray = (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));
            return new PropertyDescriptorCollection(propertyArray);
        }

        public AttributeCollection GetAttributes()
        {
           return new AttributeCollection(null);
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            return null;
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] someAttributes)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }
    }
}
