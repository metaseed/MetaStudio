using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using System.Windows.Markup;
namespace Metaseed.Windows.Media
{
    public static class FindVisualDecendant
    {
        static public TChildType FindVisualChild<TChildType>(DependencyObject obj) where TChildType : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is TChildType)
                    return (TChildType)child;
                else
                {
                    var childOfChild = FindVisualChild<TChildType>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        static public List<TChildType> FindVisualChildrenOfOneElement<TChildType>(DependencyObject obj) where TChildType : DependencyObject
        {
            var children = new List<TChildType>();
            var noneTypeChildren = new List<DependencyObject>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null)
                {
                    if (child is TChildType)
                    {
                        children.Add((TChildType)child);
                    }
                    else
                    {
                        noneTypeChildren.Add(child);
                    }
                }
            }
            if (children.Count == 0 && noneTypeChildren.Count!=0)
            {
                foreach (var item in noneTypeChildren)
                {
                    List<TChildType> childrenOfChild = FindVisualChildrenOfOneElement<TChildType>(item);
                    if (childrenOfChild.Count != 0)
                        return childrenOfChild;
                }
            }
            return children;
        }

        public static object[] FindControls(this FrameworkElement f, Type childType, int maxDepth)
        {
            return RecursiveFindControls(f, childType, 1, maxDepth);
        }
        private static object[] RecursiveFindControls(object o, Type childType, int depth, int maxDepth = 0)
        {
            var list = new List<object>();
            var attrs = o.GetType().GetCustomAttributes(typeof(ContentPropertyAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                string childrenProperty = (attrs[0] as ContentPropertyAttribute).Name;
                foreach (var c in (IEnumerable)o.GetType().GetProperty(childrenProperty).GetValue(o, null))
                {
                    if (c.GetType().FullName == childType.FullName)
                        list.Add(c); if (maxDepth == 0 || depth < maxDepth)
                        list.AddRange(RecursiveFindControls(c, childType, depth + 1, maxDepth));
                }
            } return list.ToArray();
        }
    }
}
