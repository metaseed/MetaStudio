using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.IoC;
using Metaseed.Windows.Controls;
using Xceed.Wpf.AvalonDock;

namespace Metaseed.MetaShell.ViewModels
{
    public class HostedProcessDocumentViewModel : DocumentBaseViewModel
    {
        public HostedProcessDocumentViewModel(Process process,HostedProcessWindow hostedProcessWindow=null)
        {
            Process = process;
            if (hostedProcessWindow == null)
            {
                ProcessWindow = new HostedProcessWindow() { Left = 0, Top = 0, Process = process };
            }
            else
            {
                ProcessWindow = hostedProcessWindow;
            }
            ProcessWindow.MouseDown += ProcessWindow_MouseDown;
        }
        void ProcessWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsFloating)
            {
                IsActive = true;
            }
        }

        protected override void OnIsFloatingChanged(bool isFloating)
        {
            base.OnIsFloatingChanged(isFloating);
            var dockingManager = this.GetDependencyResolver().Resolve<DockingManager>();
            dockingManager.FloatingWindows.ToList().ForEach(window =>
            {
                window.ShowInTaskbar = true;
            });

        }

        public Process Process { get; private set; }
        private HostedProcessWindow _hostedProcessWindow;
        public HostedProcessWindow ProcessWindow
        {
            get
            {
                return _hostedProcessWindow;
            }
            private set
            {
                _hostedProcessWindow = value;
            }
        } 
    }
          


}
