using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace Metaseed.Collections.Generic
{
    /// <summary>
    /// LINQ extensions for IEnumerable
    /// </summary>
    public static class IEnumerableExtensions
    {
        
        /// <summary>
        /// the default ICollection<T> Contains using Equsls which may cause problem when using with Catel ModelBase class:
        /// /// <summary>
        /// the default equals methord of Catel.Data.ModelBase compares the properties(PropertyData) in it's PropertyBag
        /// when not using PropertyData to define property PropertyBag the bag is empty and the same type object will be equal
        /// //https://catelproject.atlassian.net/browse/CTL-178
        /// not sure whether or not the property bag is the source of the problem after I creat a PropertData property with different default valus and the problem is the same.
        //what I do:
        //ViewModelBaseDerivedTypeA a1,a2;
        //ObservableCollection<ViewModelBaseDerivedTypeA> C;
        //then I add a1, a2.
        //void addToC(ViewModelBaseDerivedTypeA a)
        //{ if(!C.Contains(a)) C.Add(a); }
        //but only a1 is added to C;
        //after I overide the Equals methord in ViewModelBaseDerivedTypeA 
        //public override bool Equals(object obj)
        //{ return this == obj; }
        //a1 a2 is added to C.
        //then what's the problem, many thanks!!
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //public override bool Equals(object obj) this method is in LayoutContentViewModels
        //{
        //    return obj==this;
        //}
        //now using Contains_CompareByReference 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Contains_CompareByReference<T>(this IEnumerable<T> collection, T item)
        {
            foreach (var document in collection)
            {
                if (object.ReferenceEquals(document, item))
                {
                    return true;
                }
            }
            return false;
        }
        //http://stackoverflow.com/questions/200574/linq-equivalent-of-foreach-for-ienumerablet?lq=1
        /// <summary>
        /// ForEach extension
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ie"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
        //please use OfType !!
        //public static IEnumerable<T> OfBaseType<T>(this IEnumerable ie)
        //{
        //    foreach (var e in ie)
        //    {
        //        typeof(T).IsAssignableFrom(e.GetType());
        //        yield return (T)e;
        //    }
        //}
        /// <summary>
        /// http://stackoverflow.com/questions/255341/getting-key-of-value-of-a-generic-dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static IEnumerable<TKey> KeysFromValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue val)
        {
            if (dict == null)
            {
                throw new ArgumentNullException("dict");
            }
            return dict.Keys.Where(k => dict[k].Equals ( val));
        }
    }
    public static class CollectionsUtil
    {
        public static List<T> EnsureSize<T>(this List<T> list, int size)
        {
            return EnsureSize(list, size, default(T));
        }

        public static List<T> EnsureSize<T>(this List<T> list, int size, T value)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (size < 0) throw new ArgumentOutOfRangeException("size");

            int count = list.Count;
            if (count < size)
            {
                int capacity = list.Capacity;
                if (capacity < size)
                    list.Capacity = Math.Max(size, capacity * 2);

                while (count < size)
                {
                    list.Add(value);
                    ++count;
                }
            }

            return list;
        }
    }

}
