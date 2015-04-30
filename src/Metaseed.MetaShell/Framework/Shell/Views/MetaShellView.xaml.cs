using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

using Catel;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.MVVM.Providers;
using Catel.MVVM.Views;
//using Microsoft.Practices.Prism.Events;
using Metaseed.ShellBase.Framework.Shell.Views;
using System.Diagnostics;
using Catel.Data;
namespace Metaseed.MetaShell.Views
{
    using ViewModels;
    using Infrastructure;
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MetaShellView : ShellViewBase
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        #endregion

        #region Constructors
        public MetaShellView() : this(null) { }
        protected virtual Type GetViewModelType() { return null; }
        static bool created = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView"/> class.
        /// </summary>
        public MetaShellView(IViewModel viewModel)
            : base(viewModel)
        {
            if (!created) { created = true; } else { throw new Exception("Shell view model creatd multitimes!"); }
            InitializeComponent();
            CustomiseMainWindowIcon("Resources\\Images\\ApplicationIcon.png");
            this.Title = "MetaStudio - Ver: " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            var serviceLocator = ServiceLocator.Default;
            serviceLocator.RegisterInstance<ShellRibbon>(Part_Ribbon);
            serviceLocator.RegisterInstance(DockingManager);
            serviceLocator.RegisterInstance(LayoutDocumentPane);

            Part_Ribbon.AutomaticStateManagement = true;
            //ribbon.EnsureTabItem("Home");
            serviceLocator.RegisterInstance(this.MyNotifyIcon);

            Loaded += (sender, e) =>
            {
                string inputFilePathName = GloableStaticInstanse.StartupInputFilePathName;
                if (!string.IsNullOrEmpty(inputFilePathName))
                {
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => ((MetaShellViewModel)ViewModel).PackageService.Open(inputFilePathName)));
                    
                }
            };
            
            this.MouseDoubleClick += ShellView_MouseDoubleClick;
        }

        void ShellView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //((ShellViewModel)ViewModel).ShellService.Documents.Clear();
        }
        #endregion

        #region Properties

        #endregion

       
        #region Methods

        #region PrivateMethods
        protected override void OnClosing(CancelEventArgs e)
        {
            var viewModel = (MetaShellViewModel)ViewModel;
            if (String.IsNullOrEmpty(viewModel.PackageService.CurrentPackagePath))
            {
                e.Cancel = false;
                AppClosingEvent.SendWith(new Tuple<IDataWindow, CancelEventArgs>(this,e));
                //eventAggregator.GetEvent<AppClosingEvent>().Publish(this);
            }
            else
            {
                var closeAppDialog = new CloseAppDialog {Topmost = true};
                closeAppDialog.ShowDialog();
                if (closeAppDialog.SelectedButton == CloseAppDialogButtons.Quit)//quit
                {
                    e.Cancel = false;
                    AppClosingEvent.SendWith(new Tuple<IDataWindow, CancelEventArgs>(this, e));
                    //eventAggregator.GetEvent<AppClosingEvent>().Publish(this);
                }
                else if (closeAppDialog.SelectedButton == CloseAppDialogButtons.SaveAndQuit) //save &quit
                {
                    viewModel.PackageService.Save(viewModel.PackageService.CurrentPackagePath);
                    e.Cancel = false;
                    AppClosingEvent.SendWith(new Tuple<IDataWindow, CancelEventArgs>(this, e));
                    //eventAggregator.GetEvent<AppClosingEvent>().Publish(this);
                }
                else//cancle
                {
                    e.Cancel = true;
                    GloableStaticInstanse.AppStatus = AppStatus.Running;
                }

            }
            base.OnClosing(e);
        }
        #endregion PrivateMethods
        #endregion

        private void CLOSE_MenuClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            zoomSlider.Value = 1;
        }

        private void StatusBarItemFilePath_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var path = ((MetaShellViewModel)(this.ViewModel)).PackageService.CurrentPackagePath;
            if (!string.IsNullOrEmpty(path))
            {
                //var folder = Catel.IO.Path.GetDirectoryName(path);
                System.Diagnostics.Process.Start("explorer.exe","/select,"+ path);
            }
            
        }
        public ShellRibbon Ribbon
        {
            get { return Part_Ribbon; }
        }
    }
}
