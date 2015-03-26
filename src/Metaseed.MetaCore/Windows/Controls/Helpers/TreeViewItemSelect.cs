using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Metaseed.Windows.Controls
{
    public static class TreeViewItemSelectHelper
    {
        /// <summary>
        /// Walks the tree items to find the node corresponding with
        /// the given item, then sets it to be selected.
        /// </summary>
        /// <param name="treeView">The tree view to set the selected
        /// item on</param>
        /// <param name="item">The item to be selected</param>
        /// <returns><c>true</c> if the item was found and set to be
        /// selected</returns>
        static public bool SetSelectedItem(
            this TreeView treeView, object item)
        {

            return SetSelected(treeView, item);
        }

        static private bool SetSelected(ItemsControl parent,
            object child)
        {

            if (parent == null || child == null)
            {
                return false;
            }

            TreeViewItem childNode = parent.ItemContainerGenerator
                .ContainerFromItem(child) as TreeViewItem;

            if (childNode != null)
            {
                childNode.Focus();
                return childNode.IsSelected = true;
            }

            if (parent.Items.Count > 0)
            {
                foreach (object childItem in parent.Items)
                {
                    ItemsControl childControl = parent
                        .ItemContainerGenerator
                        .ContainerFromItem(childItem)
                        as ItemsControl;

                    if (SetSelected(childControl, child))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
