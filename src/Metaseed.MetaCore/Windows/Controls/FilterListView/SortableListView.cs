using System;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using Metaseed.Windows.Media;
using System.Windows.Media;
using System.Collections.Generic;
// SortableListView, from the following blog post:
//
// http://blogs.interknowlogy.com/joelrumerman/archive/2007/04/03/12497.aspx

namespace Metaseed.Windows.Controls
{
    /// <summary>
    /// Extends ListView to provide sortable columns
    /// </summary>
    public class SortableListView : ListView
    {
        //SortableGridViewColumn lastSortedOnColumn = null;

        //GridViewColumnHeader lastSortedOnColumnHeader = null;

        // ListSortDirection lastDirection = ListSortDirection.Ascending;
        protected ResourceDictionary dictionary;


        #region New Dependency Properties


        //public string ColumnHeaderSortedAscendingTemplate
        //{
        //    get { return (string)GetValue(ColumnHeaderSortedAscendingTemplateProperty); }
        //    set { SetValue(ColumnHeaderSortedAscendingTemplateProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ColumnHeaderSortedAscendingTemplate.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ColumnHeaderSortedAscendingTemplateProperty =
        //               DependencyProperty.Register("ColumnHeaderSortedAscendingTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(""));


        //public string ColumnHeaderSortedDescendingTemplate
        //{
        //    get { return (string)GetValue(ColumnHeaderSortedDescendingTemplateProperty); }
        //    set { SetValue(ColumnHeaderSortedDescendingTemplateProperty, value); }
        //}


        //// Using a DependencyProperty as the backing store for ColumnHeaderSortedDescendingTemplate.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ColumnHeaderSortedDescendingTemplateProperty =
        //    DependencyProperty.Register("ColumnHeaderSortedDescendingTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(""));


        //public string ColumnHeaderNotSortedTemplate
        //{
        //    get { return (string)GetValue(ColumnHeaderNotSortedTemplateProperty); }
        //    set { SetValue(ColumnHeaderNotSortedTemplateProperty, value); }
        //}


        //// Using a DependencyProperty as the backing store for ColumnHeaderNotSortedTemplate.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ColumnHeaderNotSortedTemplateProperty =
        //    DependencyProperty.Register("ColumnHeaderNotSortedTemplate", typeof(string), typeof(SortableListView), new UIPropertyMetadata(""));

        #endregion



        ///
        /// Executes when the control is initialized completely the first time through. Runs only once.
        ///
        ///
        protected override void OnInitialized(EventArgs e)
        {
            Uri uri = new Uri("/Metaseed.MetaCore;component/Windows/Controls/FilterListView/FilterListViewDictionary.xaml", UriKind.Relative);
            dictionary = Application.LoadComponent(uri) as ResourceDictionary;
            // add the event handler to the GridViewColumnHeader. This strongly ties this ListView to a GridView.
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
            this.Loaded += new RoutedEventHandler(SortableListView_Loaded);
            base.OnInitialized(e);
        }
        //http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/67cdbaa0-d28a-4a16-8f01-8862469f220d/
        private void FindAllElements2(Type T, Visual root, List<Visual> elementList)
        {
            if (root == null)
                return;
            //Trace.TraceInformation("FindAllElements: {0}", root.GetType().Name);
            if (T.Equals(root.GetType()))
            {
                elementList.Add(root);
                return;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                Visual child = VisualTreeHelper.GetChild(root, i) as Visual;
                FindAllElements2(T, child, elementList);
            }
        }
        Dictionary<GridViewColumn, GridViewColumnHeader> m_Headers = null;

        private void FindTheHeaders()
        {
            List<Visual> elementList = new List<Visual>();
            FindAllElements2(typeof(GridViewColumnHeader), this, elementList);
            m_Headers = new Dictionary<GridViewColumn, GridViewColumnHeader>();
            foreach (Visual element in elementList)
            {
                GridViewColumnHeader header = element as GridViewColumnHeader;
                if (header.Column != null)
                {
                    //string key = (header.Column.DisplayMemberBinding as Binding).Path.Path as string;
                    GridViewColumn key = header.Column;
                    m_Headers.Add(key, header);
                }
            }
        }


        void SortableListView_Loaded(object sender, RoutedEventArgs e)
        {
            FindTheHeaders();
            // cast the ListView's View to a GridView
            GridView gridView = this.View as GridView;
            if (gridView != null)
            {
                // determine which column is marked as IsDefaultSortColumn. Stops on the first column marked this way.
                SortableGridViewColumn sortableGridViewColumn = null;
                foreach (GridViewColumn gridViewColumn in gridView.Columns)
                {
                    sortableGridViewColumn = gridViewColumn as SortableGridViewColumn;
                    if (sortableGridViewColumn != null)
                    {
                        if (sortableGridViewColumn.SortDirection != SortableGridViewColumn.NoSort && !string.IsNullOrEmpty(sortableGridViewColumn.SortPropertyName))
                        {
                            var dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                            if (dataView != null)
                            {
                                dataView.SortDescriptions.Add(new SortDescription(sortableGridViewColumn.SortPropertyName, sortableGridViewColumn.SortDirection));
                                GridViewColumnHeader header = m_Headers[sortableGridViewColumn];
                                if (header != null)
                                {
                                    ListSortDirection direction = sortableGridViewColumn.SortDirection;
                                    Label sortIndicator = (Label)BaseWPFHelpers.SingleFindDownInTree(header, new BaseWPFHelpers.FinderMatchName("sortIndicator"));
                                    if (direction == SortableGridViewColumn.NoSort)
                                    {
                                        sortIndicator.Style = (Style)dictionary["HeaderTemplateTransparent"];
                                    }
                                    else
                                    {
                                        if (direction == ListSortDirection.Ascending)
                                        {
                                            sortIndicator.Style = (Style)dictionary["HeaderTemplateArrowUp"];
                                        }
                                        else
                                        {
                                            sortIndicator.Style = (Style)dictionary["HeaderTemplateArrowDown"];
                                        }
                                    }
                                }

                                //if (sortableGridViewColumn.SortDirection == ListSortDirection.Ascending)
                                //{
                                //    if (!String.IsNullOrEmpty(this.ColumnHeaderSortedAscendingTemplate))
                                //    {
                                //        sortableGridViewColumn.HeaderTemplate = this.TryFindResource(ColumnHeaderSortedAscendingTemplate) as DataTemplate;
                                //    }
                                //}
                                //else
                                //{
                                //    if (!String.IsNullOrEmpty(this.ColumnHeaderSortedDescendingTemplate))
                                //    {
                                //        sortableGridViewColumn.HeaderTemplate = this.TryFindResource(ColumnHeaderSortedDescendingTemplate) as DataTemplate;
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
        }





        ///
        /// Event Handler for the ColumnHeader Click Event.
        ///
        ///
        ///
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            // ensure that we clicked on the column header and not the padding that's added to fill the space.
            if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                SortableGridViewColumn sortableGridViewColumn = (headerClicked.Column) as SortableGridViewColumn;
                // attempt to cast to the sortableGridViewColumn object.
                var dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                // ensure that the column header is the correct type and a sort property has been set.
                if (sortableGridViewColumn != null)
                {
                    // get the sort property name from the column's information.
                    string sortPropertyName = sortableGridViewColumn.SortPropertyName;
                    if (!String.IsNullOrEmpty(sortPropertyName))
                    {
                        var lastDirection = sortableGridViewColumn.SortDirection;
                        ListSortDirection direction = SortableGridViewColumn.NoSort;
                        // determine if this is a new sort, or a switch in sort direction.
                        if (lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else if (lastDirection == ListSortDirection.Descending)
                        {
                            direction = SortableGridViewColumn.NoSort;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                        Label sortIndicator = (Label)BaseWPFHelpers.SingleFindDownInTree(headerClicked, new BaseWPFHelpers.FinderMatchName("sortIndicator"));
                        // Sort the data.
                        Sort(sortPropertyName, direction);
                        if (direction == SortableGridViewColumn.NoSort)
                        {
                            sortIndicator.Style = (Style)dictionary["HeaderTemplateTransparent"];
                        }
                        else
                        {
                            if (direction == ListSortDirection.Ascending)
                            {
                                sortIndicator.Style = (Style)dictionary["HeaderTemplateArrowUp"];
                            }
                            else
                            {
                                sortIndicator.Style = (Style)dictionary["HeaderTemplateArrowDown"];
                            }
                        }
                        sortableGridViewColumn.SortDirection = direction;
                    }
                }
            }
        }


        ///
        /// Helper method that sorts the data.
        ///
        ///
        ///
        private void Sort(string sortBy, ListSortDirection direction)
        {
            var dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);
            for (int i = 0; i < dataView.SortDescriptions.Count; i++)
            {
                if (sortBy.Equals(dataView.SortDescriptions[i].PropertyName))
                {
                    dataView.SortDescriptions.RemoveAt(i);
                    break;
                }
            }
            if (direction != SortableGridViewColumn.NoSort)
            {
                SortDescription sd = new SortDescription(sortBy, direction);
                dataView.SortDescriptions.Add( sd);
            }
            //lastDirection = direction;
            dataView.Refresh();
        }

    }
}
