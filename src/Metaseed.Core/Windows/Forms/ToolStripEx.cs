using System;
using System.Windows.Forms;

namespace Metaseed.Windows.Forms
{
    /// <summary>
    /// used to make the buttons in toolstip clickable, when the toolstrip in other process window is docked into MetaStudio tabbed window.
    /// Note: items of the default windows form toolstrip control is not clickable when docked as MetaStudio tabbed window.
    /// this code snippet is tacken from Metaseed.Core lib.
    /// </summary>
    public partial class ToolStripEx : ToolStrip
    {
        private bool clickThrough = true;

        /// <summary>
        /// http://blogs.msdn.com/b/rickbrew/archive/2006/01/09/511003.aspx
        /// Gets or sets whether the ToolStripEx honors item clicks when its containing form does
        /// not have input focus.
        /// </summary>
        /// <remarks>
        /// Default value is false, which is the same behavior provided by the base ToolStrip class.
        /// </remarks>
        public bool ClickThrough
        {
            get { return this.clickThrough; }

            set { this.clickThrough = value; }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!this.clickThrough)
                return;
            ClickThroughWndProc(ref m);
        }

        internal static bool ClickThroughWndProc(ref Message m)
        {
            bool flag = false;
            if (m.Msg == NativeConstants.WM_MOUSEACTIVATE &&
                m.Result == (IntPtr)NativeConstants.MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr)NativeConstants.MA_ACTIVATE;
                flag = true;
            }
            return flag;
        }
    }

    internal sealed class NativeConstants
    {
        internal const uint WM_MOUSEACTIVATE = 0x21;
        internal const uint MA_ACTIVATE = 1;
        internal const uint MA_ACTIVATEANDEAT = 2;
        internal const uint MA_NOACTIVATE = 3;
        internal const uint MA_NOACTIVATEANDEAT = 4;
    }
}


