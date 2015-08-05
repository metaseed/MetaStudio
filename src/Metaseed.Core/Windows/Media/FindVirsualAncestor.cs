using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using System.Windows.Markup;
namespace Metaseed.Windows.Media
{
    public static class FindVirsualAncestor
    {
        /// <summary>
        /// Finds the visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindVisualAncestor(this DependencyObject startElement, Predicate<object> condition, int maxDepth = -1)
        {
            var dependencyObject = startElement;
            while (dependencyObject != null && !condition(dependencyObject))
            {
                if (maxDepth == 0)
                {
                    return null;
                }
                if (maxDepth > 0)
                {
                    maxDepth--;
                }
                dependencyObject = dependencyObject.GetVisualParent();
            }
            return dependencyObject;
        }
        //use visual. public bool IsAncestorOf(DependencyObject descendant);
        //public static bool IsVisualAncestor(this DependencyObject decendant,DependencyObject ancestor, int maxDepth = -1)
        //{
        //    var foundAncestor =decendant.FindVisualAncestor((dependencyObject) => dependencyObject == ancestor,maxDepth);
        //    return foundAncestor != null;
        //}
        public static T FindVisualAncestor<T>(this DependencyObject startElement, int maxDepth = -1) where T : class
        {
            var reference = startElement;
            do
            {
                if (maxDepth == 0)
                {
                    return null;
                }
                if (maxDepth > 0)
                {
                    maxDepth--;
                }
                reference = reference.GetVisualParent();
            }
            while (reference != null && !(reference is T));
            return reference as T;
        }
        public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            DependencyObject reference = dependencyObject;
            do
            {
                reference = reference.GetVisualParent();
            }
            while (reference != null && !(reference is T));
            return reference as T;
        }
        /// <summary>   
        /// Finds a parent of a given item on the visual tree.   
        /// </summary>   
        /// <typeparam name="TParentType">The type of the queried item.</typeparam>   
        /// <param name="child">A direct or indirect child of the queried item.</param>   
        /// <returns>
        /// The first parent item that matches the submitted type parameter.    
        /// If not matching item can be found, a null reference is being returned.
        /// </returns>   
        public static TParentType FindAncestor<TParentType>(DependencyObject child) where TParentType : DependencyObject
        {  
            var parentObject = VisualTreeHelper.GetParent(child);
            // we’ve reached the end of the tree      
            if (parentObject == null) return null;
            // check if the parent matches the type we’re looking for      
            var parent = parentObject as TParentType;
            return parent ?? FindAncestor<TParentType>(parentObject);
        }
        /// <summary>   
        /// Finds a parent of a given item on the visual tree.   
        /// </summary>    
        /// <typeparam name="T">The type of the queried item.</typeparam>    
        /// <param name="child">A direct or indirect child of the    
        /// queried item.</param>    
        /// <returns>The first parent item that matches the submitted    
        /// type parameter. If not matching item can be found, a null    
        /// reference is being returned.</returns>    
        public static T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = GetVisualParent(child);
            //we've reached the end of the tree     
            if (parentObject == null) return null;
            //check if the parent matches the type we're looking for   
            var parent = parentObject as T;
            return parent ?? TryFindParent<T>(parentObject);
        }
        /// <summary>    
        /// This method is an alternative to WPF's    
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also    
        /// supports content elements. Do note, that for content element,    
        /// this method falls back to the logical tree of the element!   
        /// /// </summary>    
        /// <param name="child">The item to be processed.</param>    
        /// <returns>The submitted item's parent, if available. Otherwise    
        /// null.</returns>   
        public static DependencyObject GetParent(DependencyObject child)
        {
            if (child == null) return null;
            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;
                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }
            //if it's not a ContentElement, rely on VisualTreeHelper    
            return VisualTreeHelper.GetParent(child);
        }
        public static DependencyObject GetVisualParent(this DependencyObject element)
        {
            return GetParent(element);
        }
    }
}
