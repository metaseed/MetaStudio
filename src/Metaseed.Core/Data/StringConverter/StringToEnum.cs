using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Data
{
   public static class StringToEnumConverter
    {
        public static object StringToEnum(Type t, string value)
        {
            //foreach (FieldInfo fi in t.GetFields())
            //    if (fi.Name.ToUpper() == value.ToUpper())
            //        return fi.GetValue(null);    // We use null because enumeration values are static

            //throw new Exception(string.Format("Can't convert {0} to {1}", value, t.ToString()));

            return (from f in t.GetFields()
                    where f.Name.ToUpper() == value.ToUpper()
                    select f.GetValue(null)).Single();
        }

        public static bool TryStringToEnum(Type t, string value)
        {
            //bool output = false;
            //foreach (FieldInfo fi in t.GetFields())
            //    if (fi.Name == value)
            //    {
            //        output = true;
            //        break;
            //    }
            //return output;

            return (from f in t.GetFields()
                    where f.Name == value
                    select f).Count() > 0;
        }

        public static T StringToEnum<T>(string value)
        {
            T output;

            output = (T)StringToEnum(typeof(T), value);

            return output;
        }

        public static bool TryStringToEnum<T>(string value)
        {
            bool output;

            output = TryStringToEnum(typeof(T), value);

            return output;
        }
        public static T ToEnum<T>(this string s)
        {
            return (T)StringToEnum(typeof(T), s);
        }

        public static bool TryToEnum<T>(this string value)
        {
            bool output;

            output = TryStringToEnum(typeof(T), value);

            return output;
        }
    }
}
