﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Fluent;
using Metaseed.Windows.Threading;

namespace Metaseed.MetaShell.Controls
{
    public class RibbonTabContextUIHelper
    {
        public static void Show(RibbonTabItem tab, Ribbon ribbon)
        {
            if (tab == null || tab.Group == null) return;
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
            tab.Group.UpdateLayout();
            if (ribbon != null && !ribbon.IsMinimized)
                tab.IsSelected = true;
        }

        public static void Hide(RibbonTabItem tab)
        {
            if (tab == null || tab.Group == null) return;
            tab.Visibility = Visibility.Collapsed;
            bool hasVisibleItem = tab.Group.Items.Any(tabItem => tabItem.Visibility == Visibility.Visible);
            if (!hasVisibleItem)
                tab.Group.Visibility = Visibility.Collapsed;
            tab.Group.UpdateLayout();
        }

    }
}
