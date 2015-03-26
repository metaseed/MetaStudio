using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Collections.Generic
{
    static public class IListExtension
    {
        //http://stackoverflow.com/questions/1945461/how-do-i-sort-an-observable-collection
        //ObservableCollection<Person> people = new ObservableCollection<Person>();
        //people.Sort(p => p.FirstName);
        public static void Sort<TSource, TKey>(this IList<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) return;
            Comparer<TKey> comparer = Comparer<TKey>.Default;
            for (int i = source.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    TSource o1 = source[j - 1];
                    TSource o2 = source[j];
                    if (comparer.Compare(keySelector(o1), keySelector(o2)) > 0)
                    {
                        source.Remove(o1);
                        source.Insert(j, o1);
                    }
                }
            }
        }
        /// <summary>
        /// http://stackoverflow.com/questions/7284805/how-to-sort-observablecollection
        /// ObservableCollection<string> strs = new ObservableCollection<string>();
        //Comparison<string> comparison = new Comparison<string>((s1, s2) => { return String.Compare(s1, s2); });
        //strs.InsertSorted("Mark", comparison);
        //strs.InsertSorted("Tim", comparison);
        //strs.InsertSorted("Joe", comparison);
        //strs.InsertSorted("Al", comparison);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="comparison"></param>
        public static void InsertSorted<T>(this IList<T> collection, T item, Comparison<T> comparison)
        {
            if (collection.Count == 0)
                collection.Add(item);
            else
            {
                bool last = true;
                for (int i = 0; i < collection.Count; i++)
                {
                    int result = comparison.Invoke(collection[i], item);
                    if (result >= 1)
                    {
                        collection.Insert(i, item);
                        last = false;
                        break;
                    }
                }
                if (last)
                    collection.Add(item);
            }
        }

    }
}
