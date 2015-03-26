using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.Windows.Controls;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Globalization;
namespace Metaseed.Windows.Controls
{
 public static class DataGridHelp
    {
     static T FindVisualParent<T>(UIElement element) where T : UIElement
     {
         UIElement parent = element;
         while (parent != null)
         {
             T correctlyTyped = parent as T;
             if (correctlyTyped != null)
             {
                 return correctlyTyped;
             }

             parent = VisualTreeHelper.GetParent(parent) as UIElement;
         }
         return null;
     } 
        #region GetCell

        public static DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);

                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }

                return cell;
            }

            return null;
        }

        #endregion GetCell

        #region GetRow

        /// <summary>
        /// Gets the DataGridRow based on the given index
        /// </summary>
        /// <param name="index">the index of the container to get</param>
        public static DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                dataGrid.UpdateLayout();

                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        #endregion GetRow

        #region GetRowHeader

        /// <summary>
        /// Gets the DataGridRowHeader based on the row index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DataGridRowHeader GetRowHeader(DataGrid dataGrid, int index)
        {
            return GetRowHeader(GetRow(dataGrid, index));
        }

        /// <summary>
        /// Returns the DataGridRowHeader based on the given row.
        /// </summary>
        /// <param name="row">Uses reflection to access and return RowHeader</param>
        public static DataGridRowHeader GetRowHeader(DataGridRow row)
        {
            if (row != null)
            {
                return GetVisualChild<DataGridRowHeader>(row);
            }
            return null;
        }

        #endregion GetRowHeader

        #region GetColumnHeader

        public static DataGridColumnHeader GetColumnHeader(DataGrid dataGrid, int index)
        {
            DataGridColumnHeadersPresenter presenter = GetVisualChild<DataGridColumnHeadersPresenter>(dataGrid);

            if (presenter != null)
            {
                return (DataGridColumnHeader)presenter.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return null;
        }

        #endregion GetColumnHeader                

        #region GetVisualChild

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        public static T GetVisualChild<T>(Visual parent, int index) where T : Visual
        {
            T child = default(T);

            int encounter = 0;
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual v = queue.Dequeue();
                child = v as T;
                if (child != null)
                {
                    if (encounter == index)
                        break;
                    encounter++;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(v);
                    for (int i = 0; i < numVisuals; i++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(v, i));
                    }
                }
            }

            return child;
        }

        public static bool VisualChildExists(Visual parent, DependencyObject visualToFind)
        {
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual v = queue.Dequeue();
                DependencyObject child = v as DependencyObject;
                if (child != null)
                {
                    if (child == visualToFind)
                        return true;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(v);
                    for (int i = 0; i < numVisuals; i++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(v, i));
                    }
                }
            }

            return false;
        }

        #endregion GetVisualChild

        #region FindPartByName

        public static DependencyObject FindPartByName(DependencyObject ele, string name)
        {
            DependencyObject result;
            if (ele == null)
            {
                return null;
            }
            if (name.Equals(ele.GetValue(FrameworkElement.NameProperty)))
            {
                return ele;
            }

            int numVisuals = VisualTreeHelper.GetChildrenCount(ele);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject vis = VisualTreeHelper.GetChild(ele, i);
                if ((result = FindPartByName(vis, name)) != null)
                {
                    return result;
                }
            }
            return null;
        }

        #endregion FindPartByName

        #region FindVisualParent

        //public static T FindVisualParent<T>(UIElement element) where T : UIElement
        //{
        //    UIElement parent = element;
        //    while (parent != null)
        //    {
        //        T correctlyTyped = parent as T;
        //        if (correctlyTyped != null)
        //        {
        //            return correctlyTyped;
        //        }

        //        parent = VisualTreeHelper.GetParent(parent) as UIElement;
        //    }

        //    return null;
        //}

        #endregion FindVisualParent

        #region WaitTillQueueItemsProcessed

        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        #endregion WaitTillQueueItemsProcessed

        public static GroupBox CreateColumnCustomizer(DataGridColumn columnSource)
        {
            TextBox displayIndexTextBox = new TextBox();
            Binding tbBinding = new Binding("DisplayIndex");
            tbBinding.Source = columnSource;
            displayIndexTextBox.SetBinding(TextBox.TextProperty, tbBinding);

            GroupBox gb_DisplayIndex = new GroupBox();
            gb_DisplayIndex.Header = "DisplayIndex:";
            gb_DisplayIndex.Content = displayIndexTextBox;

            CheckBox canSortCB = new CheckBox();
            canSortCB.Content = "CanUserSort";
            Binding binding = new Binding("CanUserSort");
            binding.Source = columnSource;
            canSortCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox canReorderCB = new CheckBox();
            canReorderCB.Content = "CanUserReorder";
            binding = new Binding("CanUserReorder");
            binding.Source = columnSource;
            canReorderCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox canResizeCB = new CheckBox();
            canResizeCB.Content = "CanUserResize";
            binding = new Binding("CanUserResize");
            binding.Source = columnSource;
            canResizeCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox isReadOnlyCB = new CheckBox();
            isReadOnlyCB.Content = "IsReadOnly";
            Binding binding1 = new Binding("IsReadOnly");
            binding1.Source = columnSource;
            isReadOnlyCB.SetBinding(CheckBox.IsCheckedProperty, binding1);

            CheckBox isFrozenCB = new CheckBox();
            isFrozenCB.Content = "IsFrozen";
            isFrozenCB.IsEnabled = false;
            binding = new Binding("IsFrozen");
            binding.Source = columnSource;
            binding.Mode = BindingMode.OneWay;
            isFrozenCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox isAutoGeneratedCB = new CheckBox();
            isAutoGeneratedCB.Content = "IsAutoGenerated";
            isAutoGeneratedCB.IsEnabled = false;
            binding = new Binding("IsAutoGenerated");
            binding.Source = columnSource;
            binding.Mode = BindingMode.OneWay;
            isAutoGeneratedCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            ComboBox sortDirectionComboBox = new ComboBox();
            sortDirectionComboBox.Items.Add("null");
            sortDirectionComboBox.Items.Add("Ascending");
            sortDirectionComboBox.Items.Add("Descending");
            Binding sortDirBinding = new Binding("SortDirection");
            sortDirBinding.Source = columnSource;
            sortDirBinding.Converter = new SortDirectionConverter();
            sortDirectionComboBox.SetBinding(ComboBox.SelectedItemProperty, sortDirBinding);
            sortDirectionComboBox.SelectedIndex = 0;

            GroupBox gb_SortDirection = new GroupBox();
            gb_SortDirection.Header = "SortDirection:";
            gb_SortDirection.Content = sortDirectionComboBox;

            ComboBox cb_Visibility = new ComboBox();
            cb_Visibility.ItemsSource = Enum.GetValues(typeof(Visibility));
            binding = new Binding("Visibility");
            binding.Source = columnSource;
            cb_Visibility.SetBinding(ComboBox.SelectedItemProperty, binding);
            cb_Visibility.SelectedIndex = 0;

            GroupBox gb_Visibility = new GroupBox();
            gb_Visibility.Header = "Visibility:";
            gb_Visibility.Content = cb_Visibility;

            TextBox widthTextBox = new TextBox();
            Binding widthBinding = new Binding("Width");
            widthBinding.Source = columnSource;
            widthTextBox.SetBinding(TextBox.TextProperty, widthBinding);

            GroupBox gb_Width = new GroupBox();
            gb_Width.Header = "Width:";
            gb_Width.Content = widthTextBox;

            TextBox actualWidthTextBox = new TextBox();
            Binding actualWidthBinding = new Binding("ActualWidth");
            actualWidthBinding.Source = columnSource;
            actualWidthBinding.Mode = BindingMode.OneWay;
            actualWidthBinding.NotifyOnTargetUpdated = true;
            actualWidthTextBox.SetBinding(TextBox.TextProperty, actualWidthBinding);

            GroupBox gbActualWidth = new GroupBox();
            gbActualWidth.Header = "ActualWidth:";
            gbActualWidth.Content = actualWidthTextBox;

            TextBox desiredWidthTextBox = new TextBox();
            Binding desiredWidthBinding = new Binding("Width.DesiredValue");
            desiredWidthBinding.Source = columnSource;
            desiredWidthBinding.Mode = BindingMode.OneWay;
            desiredWidthTextBox.SetBinding(TextBox.TextProperty, desiredWidthBinding);

            GroupBox gb_DesiredWidth = new GroupBox();
            gb_DesiredWidth.Header = "DesiredWidth:";
            gb_DesiredWidth.Content = desiredWidthTextBox;

            TextBox displayWidthTextBox = new TextBox();
            Binding displayWidthBinding = new Binding("Width.DisplayValue");
            displayWidthBinding.Source = columnSource;
            displayWidthBinding.Mode = BindingMode.OneWay;
            displayWidthTextBox.SetBinding(TextBox.TextProperty, displayWidthBinding);

            GroupBox gb_DisplayWidth = new GroupBox();
            gb_DisplayWidth.Header = "DisplayWidth:";
            gb_DisplayWidth.Content = displayWidthTextBox;

            TextBox minWidthTextBox = new TextBox();
            Binding minWidthBinding = new Binding("MinWidth");
            minWidthBinding.Source = columnSource;
            minWidthTextBox.SetBinding(TextBox.TextProperty, minWidthBinding);

            GroupBox gb_MinWidth = new GroupBox();
            gb_MinWidth.Header = "MinWidth:";
            gb_MinWidth.Content = minWidthTextBox;

            TextBox maxWidthTextBox = new TextBox();
            Binding maxWidthBinding = new Binding("MaxWidth");
            maxWidthBinding.Source = columnSource;
            maxWidthTextBox.SetBinding(TextBox.TextProperty, maxWidthBinding);

            GroupBox gb_MaxWidth = new GroupBox();
            gb_MaxWidth.Header = "MaxWidth:";
            gb_MaxWidth.Content = maxWidthTextBox;

            TextBox sortMemberPathTextBox = new TextBox();
            binding = new Binding("SortMemberPath");
            binding.Source = columnSource;
            sortMemberPathTextBox.SetBinding(TextBox.TextProperty, binding);

            GroupBox gb_SortMemberPath = new GroupBox();
            gb_SortMemberPath.Header = "SortMemberPath:";
            gb_SortMemberPath.Content = sortMemberPathTextBox;

            StackPanel sp = new StackPanel();
            sp.Children.Add(gb_DisplayIndex);
            sp.Children.Add(canSortCB);
            sp.Children.Add(canReorderCB);
            sp.Children.Add(canResizeCB);
            sp.Children.Add(isReadOnlyCB);
            sp.Children.Add(isFrozenCB);
            sp.Children.Add(isAutoGeneratedCB);
            sp.Children.Add(gb_SortDirection);
            sp.Children.Add(gb_Width);
            //sp.Children.Add(gb_DesiredWidth);
            //sp.Children.Add(gb_DisplayWidth);
            //sp.Children.Add(gbActualWidth);
            sp.Children.Add(gb_MinWidth);
            sp.Children.Add(gb_MaxWidth);
            sp.Children.Add(gb_SortMemberPath);
            sp.Children.Add(gb_Visibility);

            GroupBox gb_all = new GroupBox();
            gb_all.Header = columnSource.Header + ":";
            gb_all.Content = sp;

            return gb_all;
        }
    }
 public class SortDirectionConverter : IValueConverter
 {
     #region IValueConverter Members

     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
     {
         if (value == null)
             return null;

         switch ((ListSortDirection)value)
         {
             case ListSortDirection.Ascending:
                 return "Ascending";
             case ListSortDirection.Descending:
                 return "Descending";
             default:
                 return null;
         }
     }

     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     {
         switch ((string)value)
         {
             case "null":
                 return null;
             case "Ascending":
                 return ListSortDirection.Ascending;
             case "Descending":
                 return ListSortDirection.Descending;
             default:
                 break;
         }

         return null;
     }

     #endregion
 }

 public class BackgroundConverter : IValueConverter
 {
     #region IValueConverter Members

     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
     {
         if (value != null)
         {
             bool boolValue = (bool)value;
             if (boolValue)
             {
                 return Brushes.Aqua;
             }
             else
             {
                 return Brushes.BlanchedAlmond;
             }
             //Person person = value as Person;              
             //if (person != null)
             //{
             //    if (person.Id % 2 == 0)
             //    {
             //        return Brushes.LightSalmon;
             //    }
             //    else
             //    {
             //        return Brushes.LightGray;
             //    }
             //}
         }
         return Brushes.LightGreen;
     }

     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     {
         throw new NotSupportedException();
     }

     #endregion
 }

 public class ItemConverter : IValueConverter
 {
     #region IValueConverter Members

     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
     {
         return CollectionView.NewItemPlaceholder;
     }

     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
     {
         throw new NotSupportedException();
     }

     #endregion
 }



}
