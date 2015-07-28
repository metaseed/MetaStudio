using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Metaseed.Windows.Forms
{
    /// <summary>
    /// used to make the buttons in PropertyGrid toolstrip clickable, when the PropertyGrid in other process window is docked into MetaStudio tabbed window.
    /// Note: toolstrip items of the default windows form toolstrip control is not clickable when docked as MetaStudio tabbed window.
    /// this code snippet is tacken from Metaseed.Core lib.
    /// </summary>
    public class PropertyGridEx : PropertyGrid
    {
        private ToolStrip toolStrip;
        public PropertyGridEx()
        {
            var originalToolStrip = (ToolStrip)this.GetType().BaseType.InvokeMember("toolStrip", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            Controls.Remove(originalToolStrip);

            toolStrip = new ToolStripEx() { ClickThrough = true };
            this.GetType().BaseType.InvokeMember("toolStrip", BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance, null, this, new object[] { toolStrip });

            toolStrip.SuspendLayout();
            toolStrip.ShowItemToolTips = true;
            toolStrip.AccessibleName = "PropertyGridToolbarAccessibleName";
            toolStrip.AccessibleRole = AccessibleRole.ToolBar;
            toolStrip.TabStop = true;
            toolStrip.AllowMerge = false;
            toolStrip.ShowItemToolTips = true;
            // This caption is for testing.
            toolStrip.Text = "PropertyGridToolBar";

            // LayoutInternal handles positioning, and for perf reasons, we manually size.
            toolStrip.Dock = DockStyle.None;
            toolStrip.AutoSize = false;
            toolStrip.TabIndex = 1;
            toolStrip.ImageScalingSize = new Size(16, 16);

            // parity with the old... 
            toolStrip.CanOverflow = false;

            // hide the grip but add in a few more pixels of padding.
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            var toolStripPadding = toolStrip.Padding;
            toolStripPadding.Left = 2;
            toolStrip.Padding = toolStripPadding;
            SetToolStripRenderer();

            var mii = this.GetType().BaseType.GetMethod("AddRefTab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            mii.Invoke(this, new object[] { DefaultTabType, null, PropertyTabScope.Static, true });
            toolStrip.ResumeLayout(false);  // SetupToolbar should perform the layout
            Controls.Add(toolStrip);
            var mi = this.GetType().BaseType.GetMethod("SetupToolbar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(bool) }, null);
            mi.Invoke(this, new object[] { true });

            this.PropertySort = PropertySort.Categorized | PropertySort.Alphabetical;
            this.Text = "PropertyGrid";
            var miii = this.GetType().BaseType.GetMethod("SetSelectState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            miii.Invoke(this, new object[] { 0 });

        }
        private void SetToolStripRenderer()
        {
            if (DrawFlatToolbar)
            {
                // use an office look and feel with system colors 
                var colorTable = new ProfessionalColorTable();
                colorTable.UseSystemColors = true;
                ToolStripRenderer = new ToolStripProfessionalRenderer(colorTable);
            }
            else
            {
                ToolStripRenderer = new ToolStripSystemRenderer();
            }
        }

    }
}
