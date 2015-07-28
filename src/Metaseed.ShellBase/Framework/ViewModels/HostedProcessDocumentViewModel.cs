using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catel.IoC;
using Metaseed.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Xceed.Wpf.AvalonDock;

namespace Metaseed.MetaShell.ViewModels
{
    public class HostedProcessDocumentViewModel : DocumentBaseViewModel
    {
        public HostedProcessDocumentViewModel(Process process, HostedProcessWindow hostedProcessWindow = null)
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
            ProcessWindow.WindowHosted += ProcessWindow_WindowHosted;
            ShowMenubarCommand = new MVVM.Commands.DelegateCommand(showMenubar);
        }
        public ICommand ShowMenubarCommand { get; set; }
        private bool _HasMenubar;

        public bool HasMenubar
        {
            get { return _HasMenubar; }
            set
            {
                _HasMenubar = value;
                RaisePropertyChanged("HasMenubar");
            }
        }

        void ProcessWindow_WindowHosted(HostedProcessWindow obj)
        {
            if (ProcessWindow.HasMenubar())
            {
                HasMenubar = true;
                //var dockingManager = this.GetDependencyResolver().Resolve<DockingManager>();
                //var menuI = new MenuItem() { Header = "Show Menu" };
                //menuI.Click += menuI_Click;
                //dockingManager.DocumentContextMenu.Items.Add(menuI);
            }
        }

        void showMenubar(object sender)
        {
            ProcessWindow.TemperaryShowMenubar(8);
        }
        void ProcessWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsActive = false;
            IsActive = true;
        }


        protected override void OnIsFloatingChanged(bool isFloating)
        {
            base.OnIsFloatingChanged(isFloating);
            if (isFloating)
            {
                var dockingManager = this.GetDependencyResolver().Resolve<DockingManager>();

                dockingManager.FloatingWindows.ToList().ForEach(window =>
                {
                    window.ShowInTaskbar = true;
                });
                ProcessWindow.Float();
            }
            else
            {
                ProcessWindow.Dock();
            }
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
        private bool _isdisposed;
        ~HostedProcessDocumentViewModel()
        {
            Dispose();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_isdisposed) return;
            if (disposing)
            {
                _isdisposed = true;

            }
            ProcessWindow.Dispose();
        }

        private ICommand _closeCommand;
        public override ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new DelegateCommand(() =>
                {
                    ShellService.CloseDocument(this);
                    Process.Kill();

                }, () => true));
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }



}
