using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Metaseed.Reflection
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// http://stackoverflow.com/questions/2656189/how-do-i-read-an-attribute-on-a-class-at-runtime
        /// usage: string name = typeof(MyClass) .GetAttributeValue((DomainNameAttribute dna) => dna.Name);
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="type"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
        static public void ChangePropertyAttributePrivateField<TProperty>(System.Linq.Expressions.Expression<Func<TProperty>> propertyExpression, Type AttributeType, String attributeFiled, object value)
        {
            string propertyName = Catel.ExpressionHelper.GetPropertyName<TProperty>(propertyExpression);
            object owner = Catel.ExpressionHelper.GetOwner<TProperty>(propertyExpression);
            if (owner == null)
            {
                //Log.Warning("ChangePropertyAttributePrivateField could not find owner of the property:" + propertyName);
                return;
            }
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(owner.GetType())[propertyName];
            var attrib = descriptor.Attributes[AttributeType];
            if (attrib == null)
            {
                //Log.Warning("ChangePropertyAttributePrivateField could not find " + AttributeType.Name + " of the property:" + propertyName);
                return;
            }
            FieldInfo privateField = attrib.GetType().GetField(attributeFiled, BindingFlags.NonPublic | BindingFlags.Instance);
            privateField.SetValue(attrib, value);
        }
    }

}
