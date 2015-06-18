using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Fluent;

namespace Metaseed.MetaShell.Controls
{
    public class RibbonTabContextUIHelper
    {
        public static void Show(RibbonTabItem tab)
        {
            if (tab.Group == null) return;
            if (tab.Group.Visibility != Visibility.Visible)
            {
                //patch to RibbonContextualTabGroup static void OnVisibilityChanged
                //because  that methord will set the tabs visibility to the group's visibility.
                var visibilityBackup = new Visibility[tab.Group.Items.Count];
                for (int i = 0; i < tab.Group.Items.Count; i++)
                {
                    visibilityBackup[i] = tab.Group.Items[i].Visibility;
                }
                //patch end
                tab.Group.Visibility = Visibility.Visible;
                //patch code
                for (int i = 0; i < tab.Group.Items.Count; i++)
                {
                    tab.Group.Items[i].Visibility = visibilityBackup[i];
                }
            }
            tab.Visibility = Visibility.Visible;
            tab.IsSelected = true;
        }

        public static void Hide(RibbonTabItem tab)
        {
            if (tab.Group == null) return;
            tab.Visibility = Visibility.Collapsed;
            foreach (var tabItem in tab.Group.Items)
            {
                if (tabItem.Visibility == Visibility.Visible) return;
            }
            tab.Group.Visibility = Visibility.Collapsed;
        }

    }
}
