using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
namespace Metaseed.Reflection
{
   public static class SetField
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="target"></param>
       /// <param name="fieldName"></param>
       /// <param name="value"></param>
       public static void SetNonPublicField(this object target, string fieldName, object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The assignment target cannot be null.");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("fieldName", "The field name cannot be null or empty.");
            }

            Type t = target.GetType();
            FieldInfo fi = null;

            while (t != null)
            {
                fi = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                if (fi != null) break;

                t = t.BaseType;
            }

            if (fi == null)
            {
                throw new Exception(string.Format("Field '{0}' not found in type hierarchy.", fieldName));
            }

            fi.SetValue(target, value);
        }
    }
}
