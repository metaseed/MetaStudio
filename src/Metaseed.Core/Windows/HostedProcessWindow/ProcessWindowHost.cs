using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Metaseed.Windows.Controls;

namespace Metaseed.Windows
{
    public class ProcessWindowHost : WindowsFormsHost
    {
        internal HostedProcessWindow ProcessWindow;
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            if (ProcessWindow != null) ProcessWindow.Update();
           return  base.ArrangeOverride(finalSize);
           
        }

        protected virtual Vector ScaleChild(Vector newScale)
        {
            return base.ScaleChild(newScale);
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }
    }
}
