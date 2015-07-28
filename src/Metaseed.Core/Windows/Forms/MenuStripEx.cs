using System;
using System.Windows.Forms;

namespace Metaseed.Windows.Forms
{
    /// <summary>
    /// used to make the items in MenuStrip clickable, when the MenuStrip in other process window is docked into MetaStudio tabbed window.
    /// Note: items of the default windows form MenuStrip control is not clickable when docked as MetaStudio tabbed window.
    /// this code snippet is tacken from Metaseed.Core lib.
    /// </summary>
    public class MenuStripEx : MenuStrip
    {
        private bool _clickThrough;
        private static int _openCount;

        public bool ClickThrough
        {
            get
            {
                return this._clickThrough;
            }
            set
            {
                this._clickThrough = value;
            }
        }

        public static bool IsAnyMenuActive
        {
            get
            {
                return MenuStripEx._openCount > 0;
            }
        }

        public MenuStripEx()
        {
            this._clickThrough = true;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!this._clickThrough)
                return;
            ToolStripEx.ClickThroughWndProc(ref m);
        }

        public static void PushMenuActivate()
        {
            ++MenuStripEx._openCount;
        }

        public static void PopMenuActivate()
        {
            --MenuStripEx._openCount;
        }

        protected override void OnMenuActivate(EventArgs e)
        {
            ++MenuStripEx._openCount;
            base.OnMenuActivate(e);
        }

        protected override void OnMenuDeactivate(EventArgs e)
        {
            --MenuStripEx._openCount;
            base.OnMenuDeactivate(e);
        }
    }
}
