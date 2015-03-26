using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.Collections.Generic
{
    public static class ArrayEqual
    {
        public static bool ArrayEquals<T>(T[] a, T[] b)
        {
            if (a.Length != b.Length)
                return false;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a.Length; i++)
            {
                if (!comparer.Equals(a[i], b[i]))
                    return false;
            }
            return true;
        }
    }
}
