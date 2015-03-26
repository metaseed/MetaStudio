#define Methord1
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Media;

namespace Metaseed.Windows.Controls
{
    /// <summary>
    /// This class contains a few useful extenders for the ListBox
    /// http://michlg.wordpress.com/2010/01/17/listbox-automatically-scroll-to-bottom/
    /// http://social.msdn.microsoft.com/Forums/en/wpf/thread/0f524459-b14e-4f9a-8264-267953418a2d
    /// </summary>
    public class ListViewExtender : DependencyObject
    {
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(ListViewExtender), new UIPropertyMetadata(default(bool),
                OnAutoScrollToEndChanged));

        /// <summary>
        /// Returns the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object whichs value should be returned</param>
        /// <returns>The value of the given property</returns>
        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// Sets the value of the AutoScrollToEndProperty
        /// </summary>
        /// <param name="obj">The dependency-object whichs value should be set</param>
        /// <param name="value">The value which should be assigned to the AutoScrollToEndProperty</param>
        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// This method will be called when the AutoScrollToEnd
        /// property was changed
        /// </summary>
        /// <param name="s">The sender (the ListBox)</param>
        /// <param name="e">Some additional information</param>
        public static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var listView = s as Control;

#if Methord2 //Methord1
            var listBoxItems = listView.Items;
            var data = listBoxItems.SourceCollection as INotifyCollectionChanged;
            var scrollToEndHandler = new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
                (s1, e1) =>
                {
                    if (listView.Items.Count > 0)
                    {
                        object lastItem = listView.Items[listView.Items.Count - 1];
                        listBoxItems.MoveCurrentTo(lastItem);
                        listView.ScrollIntoView(lastItem);
                    }
                });

            if ((bool)e.NewValue)
                data.CollectionChanged += scrollToEndHandler;
            else
                data.CollectionChanged -= scrollToEndHandler;
#else
            var scrollChangedEventHander = new System.Windows.Controls.ScrollChangedEventHandler(
                (s2, e2) =>
                {
                    if (e2.ExtentHeightChange > 0.0)
                        ((ScrollViewer)e2.OriginalSource).ScrollToEnd();
                    e2.Handled = true;
                });
            if ((Boolean)e.NewValue)
            {
                listView.AddHandler(ScrollViewer.ScrollChangedEvent, scrollChangedEventHander);
            }
            else
            {
                listView.RemoveHandler(ScrollViewer.ScrollChangedEvent, scrollChangedEventHander);
            }
#endif
        }
        //http://stackoverflow.com/questions/211971/scroll-wpf-listview-to-specific-line
        //http://stackoverflow.com/questions/1033841/is-it-possible-to-implement-smooth-scroll-in-a-wpf-listview
        //http://stackoverflow.com/questions/1009036/how-can-i-programmatically-scroll-a-wpf-listview
        //http://stackoverflow.com/questions/876994/in-wpf-how-do-i-adjust-the-scroll-increment-for-a-flowdocumentreader-with-viewin/973500#973500




        public static bool GetIsSmoothScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSmoothScrollProperty);
        }

        public static void SetIsSmoothScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSmoothScrollProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSmoothScroll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSmoothScrollProperty =
            DependencyProperty.RegisterAttached("IsSmoothScroll", typeof(bool), typeof(ListViewExtender), new UIPropertyMetadata(default(bool), OnIsSmoothScrollChanged
                ));
        public static void OnIsSmoothScrollChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ListView uiListView = s as ListView;
            var mouseDownHander = new MouseButtonEventHandler(
                (s1, e1) =>
                {
                    if (e1.MiddleButton == MouseButtonState.Pressed)
                    {
                        myMousePlacementPoint = uiListView.PointToScreen(Mouse.GetPosition(uiListView));
                    }
                });
            var mouseMoveHander=new MouseEventHandler((s2, e2) =>
            {
                ScrollViewer scrollViewer = GetScrollViewer(uiListView) as ScrollViewer;

                if (e2.MiddleButton == MouseButtonState.Pressed)
                {
                    var currentPoint = uiListView.PointToScreen(Mouse.GetPosition(uiListView));

                    if (currentPoint.Y < myMousePlacementPoint.Y)
                    {
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 3);
                    }
                    else if (currentPoint.Y > myMousePlacementPoint.Y)
                    {
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 3);
                    }

                    if (currentPoint.X < myMousePlacementPoint.X)
                    {
                        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - 3);
                    }
                    else if (currentPoint.X > myMousePlacementPoint.X)
                    {
                        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + 3);
                    }
                }
                //e2.Handled = true;
            });
            if ((Boolean)e.NewValue)
            {
                ScrollViewer.SetCanContentScroll(uiListView, true);
                uiListView.MouseDown += mouseDownHander;
                uiListView.MouseMove += mouseMoveHander;
            }
            else
            {
                ScrollViewer.SetCanContentScroll(uiListView, false);
                uiListView.MouseDown -= mouseDownHander;
                uiListView.MouseMove -= mouseMoveHander;
            }
        }
        static private Point myMousePlacementPoint;

        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer 
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }

    }



}
