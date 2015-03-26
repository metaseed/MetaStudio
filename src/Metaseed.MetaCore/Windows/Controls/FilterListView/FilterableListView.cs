using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Metaseed.Windows.Media;
using System.Collections.ObjectModel;

namespace Metaseed.Windows.Controls
{
    public class FilterItem : IComparable, INotifyPropertyChanged
    {
        static public string Null = "[empty]";
        /// <summary>
        /// The filter item instance
        /// </summary>
        private Object item;

        public Object Item
        {
            get { return item; }
            set { item = value; }
        }
        Boolean _IsChecked = false;
        public Boolean IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (value != _IsChecked)
                {
                    _IsChecked = value;
                    NotifyPropertyChanged("IsChecked");
                }

            }
        }
        /// <summary>
        /// The item viewed in the filter drop down list. Typically this is the same as the item
        /// property, however if item is null, this has the value of "[empty]"
        /// </summary>
        private Object itemView;

        public Object ItemView
        {
            get { return itemView; }
            set { itemView = value; }
        }

        public FilterItem(IComparable item)
        {
            this.item = this.itemView = item;
            if (item == null)
            {
                itemView = Null;
            }
        }

        public override int GetHashCode()
        {
            return item != null ? item.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            FilterItem otherItem = obj as FilterItem;
            if (otherItem != null)
            {
                if (otherItem.Item.Equals(this.Item))
                {
                    return true;
                }
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            FilterItem otherFilterItem = (FilterItem)obj;

            if (this.Item == null && obj == null)
            {
                return 0;
            }
            else if (otherFilterItem.Item != null && this.Item != null)
            {
                return ((IComparable)item).CompareTo((IComparable)otherFilterItem.item);
            }
            else
            {
                return -1;
            }
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// Extends ListView to provide filterable columns
    /// </summary>
    public class FilterableListView : SortableListView
    {
        #region dependency properties

        /// <summary>
        /// The style applied to the filter button when it is an active state
        /// </summary>
        public Style FilterButtonActiveStyle
        {
            get { return (Style)GetValue(FilterButtonActiveStyleProperty); }
            set { SetValue(FilterButtonActiveStyleProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonActiveStyleProperty =
                       DependencyProperty.Register("FilterButtonActiveStyle", typeof(Style), typeof(FilterableListView), new UIPropertyMetadata(null));

        /// <summary>
        /// The style applied to the filter button when it is an inactive state
        /// </summary>
        public Style FilterButtonInactiveStyle
        {
            get { return (Style)GetValue(FilterButtonInactiveStyleProperty); }
            set { SetValue(FilterButtonInactiveStyleProperty, value); }
        }

        public static readonly DependencyProperty FilterButtonInactiveStyleProperty =
                       DependencyProperty.Register("FilterButtonInActiveStyle", typeof(Style), typeof(FilterableListView), new UIPropertyMetadata(null));

        #endregion

        public static readonly ICommand ShowFilter = new RoutedCommand();
        public static readonly ICommand RemoveFilters = new RoutedCommand();
        public static readonly ICommand RefreshItems = new RoutedCommand();
        #region inner classes

        /// <summary>
        /// A simple data holder for passing information regarding filter clicks
        /// </summary>
        struct FilterStruct
        {
            public Button button;
            public FilterItem value;
            public String property;

            public FilterStruct(String property, Button button, FilterItem value)
            {
                this.value = value;
                this.button = button;
                this.property = property;
            }
        }

        /// <summary>
        /// The items which are bound to the drop down filter list
        /// </summary>


        #endregion

        private Dictionary<string, List<object>> currentFilters = new Dictionary<string, List<object>>();

        public FilterableListView()
        {
            CommandBindings.Add(new CommandBinding(ShowFilter, ShowFilterCommand));
            CommandBindings.Add(new CommandBinding(RemoveFilters, RemoveFiltersCommand));
            CommandBindings.Add(new CommandBinding(RefreshItems, RefreshItemsCommand));
        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Uri uri = new Uri("/Metaseed.MetaCore;component/Windows/Controls/FilterListView/FilterListViewDictionary.xaml", UriKind.Relative);
            dictionary = Application.LoadComponent(uri) as ResourceDictionary;

            // cast the ListView's View to a GridView
            GridView gridView = this.View as GridView;
            if (gridView != null)
            {
                //set the template
                GridViewColumnWithFilterAndSorter tempColum;
                foreach (GridViewColumn gridViewColumn in gridView.Columns)
                {
                    tempColum = gridViewColumn as GridViewColumnWithFilterAndSorter;

                    if (tempColum != null)//could be GridViewColumn
                    {
                        if (!String.IsNullOrEmpty(tempColum.SortPropertyName))//not the default value
                        {
                            if (tempColum.IsFilterable == true)
                            {
                                // apply the data template, that includes the popup, button etc ... to each column
                                gridViewColumn.HeaderTemplate = (DataTemplate)dictionary["FilterGridHeaderTemplate"];
                            }
                            else
                            {
                                // apply the data template, that includes sortable indication
                                gridViewColumn.HeaderTemplate = (DataTemplate)dictionary["SortableGridHeaderTemplate"];
                            }
                        }
                    }
                }
            }

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // ensure that the custom inactive style is applied
            if (FilterButtonInactiveStyle != null)
            {
                List<FrameworkElement> columnHeaders = BaseWPFHelpers.FindElementsOfType(this, typeof(GridViewColumnHeader));

                foreach (FrameworkElement columnHeader in columnHeaders)
                {
                    Button button = (Button)BaseWPFHelpers.FindElementOfType(columnHeader, typeof(Button));
                    if (button != null)
                    {
                        button.Style = FilterButtonInactiveStyle;
                    }
                }
            }
        }


        void ReBuildFiltersList(string propertyName, ObservableCollection<FilterItem> filterItems)
        {
            if (((ICollection)(this.ItemsSource)).Count == 0 || filterItems == null)//no itmes, no need to filter!
            {
                return;
            }
            if (Items.Count==0)
            {
                return;
            }
            bool containsNull = false;
            PropertyDescriptor filterPropDesc = TypeDescriptor.GetProperties(Items[0].GetType())[propertyName];
            // iterate over all the objects in the list
            foreach (Object item in ItemsSource)
            {
                object value = filterPropDesc.GetValue(item);
                if (value != null)
                {
                    FilterItem filterItem = new FilterItem(value as IComparable);
                    if (!filterItems.Contains(filterItem))
                    {
                        filterItems.Add(filterItem);
                    }
                }
                else
                {
                    containsNull = true;
                }
            }
            if (containsNull)
            {
                filterItems.Add(new FilterItem(null));
            }
        }
        private void RemoveFiltersCommand(object sender, ExecutedRoutedEventArgs e)
        {
            GridViewColumnHeader header = (GridViewColumnHeader)BaseWPFHelpers.FindElementOfTypeUp((MenuItem)e.OriginalSource, typeof(GridViewColumnHeader));
            GridViewColumnWithFilterAndSorter column = (GridViewColumnWithFilterAndSorter)header.Column;
            Popup popup = (Popup)BaseWPFHelpers.FindElementOfType(header, typeof(Popup));
            //foreach (var filterItem in column.FilterItems)
            //{
            //    filterItem.IsChecked = false;
            //}
            column.FilterItems = null;
            popup.IsOpen = false;
        }
        private void RefreshItemsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            GridViewColumnHeader header = (GridViewColumnHeader)BaseWPFHelpers.FindElementOfTypeUp((MenuItem)e.OriginalSource, typeof(GridViewColumnHeader));
            Popup popup = (Popup)BaseWPFHelpers.FindElementOfType(header, typeof(Popup));
            GridViewColumnWithFilterAndSorter column = (GridViewColumnWithFilterAndSorter)header.Column;
            ReBuildFiltersList(column.SortPropertyName, (ObservableCollection<FilterItem>)popup.DataContext);
            //popup.IsOpen = false;
        }
        /// <summary>
        /// Handles the ShowFilter command to populate the filter list and display the popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowFilterCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null)
            {
                // navigate up to the header
                GridViewColumnHeader header = (GridViewColumnHeader)BaseWPFHelpers.FindElementOfTypeUp(button, typeof(GridViewColumnHeader));
                // then down to the popup
                Popup popup = (Popup)BaseWPFHelpers.FindElementOfType(header, typeof(Popup));
                if (popup != null)
                {
                    // find the property name that we are filtering
                    GridViewColumnWithFilterAndSorter column = GetColumnFilters(header);
                    // open the popup to display this list
                    popup.Closed += new EventHandler(popup_Closed);
                    popup.DataContext = column.FilterItems;
                    
                    popup.IsOpen = true;
                }
            }
        }

        private GridViewColumnWithFilterAndSorter GetColumnFilters(GridViewColumnHeader header)
        {
            GridViewColumnWithFilterAndSorter column = (GridViewColumnWithFilterAndSorter)header.Column;
            String propertyName = column.SortPropertyName;
            // clear the previous filter
            if (column.FilterItems == null)
            {
                column.FilterItems = new ObservableCollection<FilterItem>();
                ReBuildFiltersList(propertyName, column.FilterItems);
            }
            CollectionViewSource.GetDefaultView(column.FilterItems).Refresh();
            return column;
        }
        public void SetFilterState(GridViewColumnHeader header, IComparable value, bool isChecked)
        {
            GridViewColumnWithFilterAndSorter column = GetColumnFilters(header);
            bool found = false;
            foreach (var filterItem in column.FilterItems)
            {
                if (filterItem.Item.Equals(value))
                {
                    found = true;
                    filterItem.IsChecked = isChecked;
                    break; ;
                }
            }
            //if (found==false)
            //{
            //    ReBuildFiltersList(column.SortPropertyName, column.FilterItems);
            //    foreach (var filterItem in column.FilterItems)
            //    {
            //        if (filterItem.Item.Equals(value))
            //        {
            //            found = true;
            //            filterItem.IsChecked = isChecked;
            //            break; ;
            //        }
            //    }
            //}
            FilterColumn(header);
        }
        void popup_Closed(object sender, EventArgs e)
        {
            Popup popup = (Popup)sender;
            // navigate up to the header to obtain the filter property name
            GridViewColumnHeader header = (GridViewColumnHeader)BaseWPFHelpers.FindElementOfTypeUp(popup, typeof(GridViewColumnHeader));
            FilterColumn(header);
            popup.Closed -= new EventHandler(popup_Closed);
        }

        private void FilterColumn(GridViewColumnHeader header)
        {
            GridViewColumnWithFilterAndSorter column = (GridViewColumnWithFilterAndSorter)header.Column;
            String currentFilterProperty = column.SortPropertyName;
            bool hasFilter = false;
            if (column.FilterItems == null)
            {
                if (currentFilters.ContainsKey(currentFilterProperty))
                {
                    currentFilters.Remove(currentFilterProperty);
                }
            }
            else
            {
                foreach (var filterItem in column.FilterItems)
                {
                    if (filterItem.IsChecked)
                    {
                        if (!currentFilters.ContainsKey(currentFilterProperty))
                        {
                            currentFilters[currentFilterProperty] = new List<object>();
                        }
                        currentFilters[currentFilterProperty].Add(filterItem.ItemView);
                        hasFilter = true;
                    }
                    else
                    {
                        if (currentFilters.ContainsKey(currentFilterProperty))
                        {
                            currentFilters[currentFilterProperty].Remove(filterItem.ItemView);
                        }
                    }
                }
            }
            // find the button and apply the active style
            Button button = (Button)BaseWPFHelpers.FindVisualElement(header, "filterButton");
            if (hasFilter)
            {
                button.ContentTemplate = (DataTemplate)dictionary["filterButtonActiveTemplate"];
                if (FilterButtonActiveStyle != null)
                {
                    button.Style = FilterButtonActiveStyle;
                }
            }
            else
            {
                currentFilters.Remove(currentFilterProperty);
                button.ContentTemplate = (DataTemplate)dictionary["filterButtonInactiveTemplate"];
                if (FilterButtonActiveStyle != null)
                {
                    button.Style = FilterButtonActiveStyle;
                }
            }
            ApplyCurrentFilters();
        }

        /// <summary>
        /// Applies the current filter to the list which is being viewed
        /// </summary>
        private void ApplyCurrentFilters()
        {

            if (Items.Count == 0)
            {
                return;
            }
            if (currentFilters.Count == 0)
            {
                Items.Filter = null;
                return;
            }
            // construct a filter and apply it               
            Items.Filter = delegate(object item)
            {
                //if (Items.Count == 0)
                //{
                //    return false;
                //}
                // when applying the filter to each item, iterate over all of
                // the current filters
                bool match = true;
                foreach (var property in currentFilters.Keys)
                {
                    bool match1 = false;
                    foreach (var value in currentFilters[property])
                    {
                        // obtain the value for this property on the item under test typeof(Employee
                        PropertyDescriptor filterPropDesc = TypeDescriptor.GetProperties(item.GetType())[property]; //TypeDescriptor.GetProperties(Items[0].GetType())[filter.property];
                        object itemValue = filterPropDesc.GetValue(item);//(Employee)

                        if (itemValue != null)
                        {
                            // check to see if it meets our filter criteria
                            if (itemValue.Equals(value))
                            {
                                match1 = true;
                                break;
                            }
                        }
                        else
                        {
                            if (value.Equals(FilterItem.Null))
                            {
                                match1 = true;
                                break;
                            }
                        }
                    }
                    if (match1 == false)
                    {
                        match = false;
                        break;
                    }
                }
                return match;
            };
        }

    }
}
