using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//http://www.codeproject.com/Articles/376643/ToolStrip-with-Custom-ToolTip
namespace Metaseed.Windows.Forms
{
    /// <summary>
    /// used to make show tooltip, when the toolstrip in other process window is docked into MetaStudio tabbed window.
    /// Note: the default windows form toolstrip control could not show tooltip when docked as MetaStudio tabbed window.
    /// this code snippet is tacken from Metaseed.Core lib.
    /// </summary>
    public partial class ToolStripEx:ToolStrip
    {
        ToolStripItem _mouseOverItem = null;
        Point _mouseOverPoint;
        readonly Timer _timer;
        public ToolTip Tooltip;
        public int ToolTipInterval = 4000;
        public string ToolTipText;
        public bool ToolTipShowUp;

        public ToolStripEx()
        {
            ShowItemToolTips = false;
            _timer = new Timer {Enabled = false, Interval = SystemInformation.MouseHoverTime};
            _timer.Tick += new EventHandler(timer_Tick);
            Tooltip = new ToolTip {ShowAlways = true};
        }

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            base.OnMouseMove(mea);
            var newMouseOverItem = this.GetItemAt(mea.Location);
            if (_mouseOverItem != newMouseOverItem ||
                (Math.Abs(_mouseOverPoint.X - mea.X) > SystemInformation.MouseHoverSize.Width || (Math.Abs(_mouseOverPoint.Y - mea.Y) > SystemInformation.MouseHoverSize.Height)))
            {
                _mouseOverItem = newMouseOverItem;
                _mouseOverPoint = mea.Location;
                if (Tooltip != null)
                    Tooltip.Hide(this);
                _timer.Stop();
                _timer.Start();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var newMouseOverItem = this.GetItemAt(e.Location);
            if (newMouseOverItem != null && Tooltip != null)
            {
                Tooltip.Hide(this);
            }
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);
            //var newMouseOverItem = this.GetItemAt(mea.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _timer.Stop();
            if (Tooltip != null)
                Tooltip.Hide(this);
            _mouseOverPoint = new Point(-50, -50);
            _mouseOverItem = null;
            //Tooltip.RemoveAll();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            var rootHwnd = GetRootHWnd(this);

            if (rootHwnd.Handle != IntPtr.Zero)
            {
                var hwndActive = GetActiveWindow();
                if (hwndActive == rootHwnd.Handle)
                {
                    return;
                }
            }
            try
            {
                Point currentMouseOverPoint;
                if (ToolTipShowUp)
                    currentMouseOverPoint = this.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y - Cursor.Current.Size.Height + Cursor.Current.HotSpot.Y));
                else
                    currentMouseOverPoint = this.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y + Cursor.Current.Size.Height - Cursor.Current.HotSpot.Y));

                if (_mouseOverItem == null)
                {
                    if (!string.IsNullOrEmpty(ToolTipText))
                    {
                        if (Tooltip == null)
                            Tooltip = new ToolTip() { ShowAlways =true};
                        //Tooltip.Show(ToolTipText, this, currentMouseOverPoint, ToolTipInterval);
                        Tooltip.SetToolTip(this, ToolTipText);
                    }
                }
                else if ((!(_mouseOverItem is ToolStripDropDownButton) && !(_mouseOverItem is ToolStripSplitButton)) ||
                    ((_mouseOverItem is ToolStripDropDownButton) && !((ToolStripDropDownButton)_mouseOverItem).DropDown.Visible) ||
                    (((_mouseOverItem is ToolStripSplitButton) && !((ToolStripSplitButton)_mouseOverItem).DropDown.Visible)))
                {
                    if (!string.IsNullOrEmpty(_mouseOverItem.ToolTipText) && Tooltip != null)
                    {
                        if (Tooltip == null)
                            Tooltip = new ToolTip() { ShowAlways = true };
                        //Tooltip.Show(mouseOverItem.ToolTipText, this, currentMouseOverPoint, ToolTipInterval);
                        Tooltip.SetToolTip(this, _mouseOverItem.AutoToolTip ? _mouseOverItem.ToolTipText : _mouseOverItem.Text);
                    }
                }
            }
            catch
            { }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _timer.Dispose();
                Tooltip.Dispose();
            }
        }
        #region pinvoke
        enum GetAncestorFlags
        {
            /// <summary>
            /// Retrieves the parent window. This does not include the owner, as it does with the GetParent function. 
            /// </summary>
            GetParent = 1,
            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GetRoot = 2,
            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent. 
            /// </summary>
            GetRootOwner = 3
        }
        static HandleRef GetRootHWnd(Control control)
        {
            return GetRootHWnd(new HandleRef(control, control.Handle));
        }
        static HandleRef GetRootHWnd(HandleRef hwnd)
        {
            IntPtr rootHwnd = GetAncestor(new HandleRef(hwnd, hwnd.Handle), GetAncestorFlags.GetRoot);
            return new HandleRef(hwnd.Wrapper, rootHwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        static extern IntPtr GetAncestor(HandleRef hWnd, GetAncestorFlags flags);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        static extern IntPtr GetActiveWindow();
        #endregion

    }
}
