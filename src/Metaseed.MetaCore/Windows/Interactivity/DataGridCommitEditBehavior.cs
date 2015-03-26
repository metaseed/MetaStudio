using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
namespace Metaseed.Windows.Interactivity
{
    /// <summary>
    /// xmlns:metaBehavior="clr-namespace:Metaseed.Windows.Interactivity;assembly=Metaseed.MetaCore"
    //Usage:<DataGrid metaBehavior:DataGridCommitEditBehavior.CommitOnLostFocus="True" />
    //http://stackoverflow.com/questions/3354916/what-is-the-proper-way-to-handle-multiple-datagrids-in-a-tab-control-so-that-cel
    ///   Provides an ugly hack to prevent a bug in the data grid.
    ///   https://connect.microsoft.com/VisualStudio/feedback/details/532494/wpf-datagrid-and-tabcontrol-deferrefresh-exception
    /// </summary>
    public class DataGridCommitEditBehavior
    {
        public static readonly DependencyProperty CommitOnLostFocusProperty =
            DependencyProperty.RegisterAttached(
                "CommitOnLostFocus",
                typeof(bool),
                typeof(DataGridCommitEditBehavior),
                new UIPropertyMetadata(false, OnCommitOnLostFocusChanged));

        /// <summary>
        ///   A hack to find the data grid in the event handler of the tab control.
        /// </summary>
        private static readonly Dictionary<Panel, DataGrid> ControlMap = new Dictionary<Panel, DataGrid>();

        public static bool GetCommitOnLostFocus(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(CommitOnLostFocusProperty);
        }

        public static void SetCommitOnLostFocus(DataGrid datagrid, bool value)
        {
            datagrid.SetValue(CommitOnLostFocusProperty, value);
        }

        private static void CommitEdit(DataGrid dataGrid)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
        }

        private static DataGrid GetParentDatagrid(UIElement element)
        {
            UIElement childElement=null; // element from which to start the tree navigation, looking for a Datagrid parent

            if (element is ComboBoxItem)
            {
                // Since ComboBoxItem.Parent is null, we must pass through ItemsPresenter in order to get the parent ComboBox
                var parentItemsPresenter = VisualTreeFinder.FindParentControl<ItemsPresenter>(element as ComboBoxItem);
                var combobox = parentItemsPresenter.TemplatedParent as ComboBox;
                childElement = combobox;
            }
            //else if (element is FrameworkElement)
            //{
            //    var fe = element as FrameworkElement;
            //    if (fe.Parent==null)
            //    {
            //        var ip = VisualTreeFinder.FindParentControl<ItemsPresenter>(fe);
            //        if (ip != null)
            //        {
            //            childElement = ip.TemplatedParent as UIElement;
            //        }
            //    }               
            //}
            else
            {
                childElement = element;
            }

            var parentDatagrid = VisualTreeFinder.FindParentControl<DataGrid>(childElement);
            return parentDatagrid;
        }

        private static Panel GetTabPanel(TabControl tabControl)
        {
            return
                (Panel)
                    tabControl.GetType().InvokeMember(
                        "ItemsHost",
                        BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
                        null,
                        tabControl,
                        null);
        }

        private static void OnCommitOnLostFocusChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = depObj as DataGrid;
            if (dataGrid == null)
            {
                return;
            }

            if (e.NewValue is bool == false)
            {
                return;
            }

            var parentTabControl = VisualTreeFinder.FindParentControl<TabControl>(dataGrid);
            Panel tabPanel=null;
            if(parentTabControl!=null){
                tabPanel = GetTabPanel(parentTabControl);
                if (tabPanel != null)
                {
                    ControlMap[tabPanel] = dataGrid;
                }
            }

            if ((bool)e.NewValue)
            {
                // Attach event handlers
                if (parentTabControl != null)
                {
                    tabPanel.PreviewMouseLeftButtonDown += OnParentTabControlPreviewMouseLeftButtonDown;
                }

                //dataGrid.LostKeyboardFocus += OnDataGridLostFocus;
                dataGrid.DataContextChanged += OnDataGridDataContextChanged;
                dataGrid.IsVisibleChanged += OnDataGridIsVisibleChanged;
            }
            else
            {
                // Detach event handlers
                if (parentTabControl != null)
                {
                    tabPanel.PreviewMouseLeftButtonDown -= OnParentTabControlPreviewMouseLeftButtonDown;
                }

                //dataGrid.LostKeyboardFocus -= OnDataGridLostFocus;
                dataGrid.DataContextChanged -= OnDataGridDataContextChanged;
                dataGrid.IsVisibleChanged -= OnDataGridIsVisibleChanged;
            }
        }

        private static void OnDataGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            CommitEdit(dataGrid);
        }

        private static void OnDataGridIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var senderDatagrid = (DataGrid)sender;

            if ((bool)e.NewValue == false)
            {
                CommitEdit(senderDatagrid);
            }
        }

        private static void OnDataGridLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            var focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement == null)
            {
                return;
            }

            var focusedDatagrid = GetParentDatagrid(focusedElement);

            // Let's see if the new focused element is inside a datagrid
            if (focusedDatagrid == dataGrid)
            {
                // If the new focused element is inside the same datagrid, then we don't need to do anything;
                // this happens, for instance, when we enter in edit-mode: the DataGrid element loses keyboard-focus, 
                // which passes to the selected DataGridCell child
                return;
            }

            CommitEdit(dataGrid);
        }

        private static void OnParentTabControlPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = ControlMap[(Panel)sender];
            CommitEdit(dataGrid);
        }
    }

    public static class VisualTreeFinder
    {
        /// <summary>
        ///   Find a specific parent object type in the visual tree
        /// </summary>
        public static T FindParentControl<T>(DependencyObject outerDepObj) where T : DependencyObject
        {
            var dObj = VisualTreeHelper.GetParent(outerDepObj);
            if (dObj == null)
            {
                return null;
            }

            if (dObj is T)
            {
                return dObj as T;
            }

            while ((dObj = VisualTreeHelper.GetParent(dObj)) != null)
            {
                if (dObj is T)
                {
                    return dObj as T;
                }
            }

            return null;
        }
    }
}
