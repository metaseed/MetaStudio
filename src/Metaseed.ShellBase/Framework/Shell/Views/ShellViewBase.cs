using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Catel;
using Catel.Data;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.MVVM.Providers;
using Catel.MVVM.Views;
using Fluent;
using Metaseed.MetaShell.Infrastructure;
using Metaseed.MetaShell.ViewModels;
using Metaseed.MetaShell.Views;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Metaseed.ShellBase.Framework.Shell.Views
{
    public class ShellViewBase : RibbonWindow, IDataWindow
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion
        private event EventHandler<EventArgs> _viewLoaded;
        private event EventHandler<EventArgs> _viewUnloaded;
        private event EventHandler<DataContextChangedEventArgs> _viewDataContextChanged;
        public ShellViewBase(IViewModel viewModel)
        {
            if (AppEnvironment.IsInDesignMode) return;
            //IRegionManager regionManager =Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IRegionManager>();
            //var viewModelType = (viewModel != null) ? viewModel.GetType() : GetViewModelType();
            //if (viewModelType == null)
            //{
            //    var viewModelLocator = ServiceLocator.Default.ResolveType<IViewModelLocator>();
            //    viewModelType = viewModelLocator.ResolveViewModel(GetType());
            //    if (viewModelType == null)
            //    {
            //        const string error = "The view model of the view could not be resolved. Use either the GetViewModelType() method or IViewModelLocator";
            //        throw new NotSupportedException(error);
            //    }
            //}
            var serviceLocator = ServiceLocator.Default;
            serviceLocator.RegisterInstance<ShellViewBase>(this);
            _windowLogic = new WindowLogic(this, null, viewModel);
            _windowLogic.ViewModelPropertyChanged += (s, e) =>
            {
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, "ActualWidth", StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, "ActualHeight", StringComparison.InvariantCulture))
                {
                    PropertyChanged.SafeInvoke(this, e);
                }
            };
            _windowLogic.ViewModelClosed += OnViewModelClosed;
            _windowLogic.ViewModelChanged += (sender, e) => RaiseViewModelChanged();
            _windowLogic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(sender, e);
                ViewModelPropertyChanged.SafeInvoke(this, e);
            };

            Loaded += (sender, e) =>
            {
                _viewLoaded.SafeInvoke(this);
                //OnLoaded(e);
            };

            Unloaded += (sender, e) =>
            {
                _viewUnloaded.SafeInvoke(this);
                //OnUnloaded(e);
            };
            SetBinding(TitleProperty, new Binding("Title"));
            Loaded += (sender, e) => Initialize();
            Closing += OnDataWindowClosing;
            DataContextChanged += (sender, e) => _viewDataContextChanged.SafeInvoke(this, new DataContextChangedEventArgs(e.OldValue, e.NewValue));

            //eventAggregator = (IEventAggregator)serviceLocator.GetService(typeof(IEventAggregator));
            


            //Loaded += (sender, e) =>
            //{
            //    string inputFilePathName = GloableStaticInstanse.StartupInputFilePathName;
            //    if (!string.IsNullOrEmpty(inputFilePathName))
            //    {
            //        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => ((MetaShellViewModel)ViewModel).PackageService.Open(inputFilePathName)));

            //    }
            //};
        }
        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        event EventHandler<DataContextChangedEventArgs> IView.DataContextChanged
        {
            add
            {
                this._viewDataContextChanged += value;
            }
            remove
            {
                this._viewDataContextChanged -= value;
            }
        }
        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        event EventHandler<EventArgs> IView.Loaded
        {
            add
            {
                this._viewLoaded += value;
            }
            remove
            {
                this._viewLoaded -= value;
            }
        }
        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        event EventHandler<EventArgs> IView.Unloaded
        {
            add
            {
                this._viewUnloaded += value;
            }
            remove
            {
                this._viewUnloaded -= value;
            }
        }
        /// <summary>
        /// Called when a property on the current view model has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //this.RaiseCanExecuteChangedForAllCommands();
            this.OnViewModelPropertyChanged(e);
        }
        /// <summary>
        /// Called when a property on the current <see cref="P:Catel.Windows.DataWindow.ViewModel" /> has changed.
        /// </summary>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Initializes the window.
        /// </summary>
        protected virtual void Initialize()
        {
            //base.Dispatcher.BeginInvoke(new Action(this.RaiseCanExecuteChangedForAllCommands), new object[0]);
        }
        /// <summary>
        /// Handles the Closing event of the DataWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="T:System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private async void OnDataWindowClosing(object sender, CancelEventArgs args)
        {
            //if (!this.ClosedByButton)
            //{
            //    await this.DiscardChanges();
            //}
        }
        /// <summary>
        /// Called when the <see cref="P:Catel.Windows.DataWindow.ViewModel" /> has changed.
        /// </summary>
        /// <remarks>
        /// This method does not implement any logic and saves a developer from subscribing/unsubscribing
        /// to the <see cref="E:Catel.Windows.DataWindow.ViewModelChanged" /> event inside the same user control.
        /// </remarks>
        protected virtual void OnViewModelChanged()
        {
        }

        private void RaiseViewModelChanged()
        {
            this.OnViewModelChanged();
            this.ViewModelChanged.SafeInvoke(this);
            this.PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs("ViewModel"));
        }
        protected virtual void OnViewModelClosed(object sender, ViewModelClosedEventArgs e)
        {
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            GloableStaticInstanse.AppStatus = AppStatus.Closing;
            base.OnClosing(e);
        }
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            GloableStaticInstanse.AppStatus = AppStatus.Running;
            if (String.IsNullOrEmpty(GloableStaticInstanse.StartupInputFilePathName))
            {
                GloableStaticInstanse.StopWatch.Stop();
            }
            //Log.Info(Properties.Resources.AppStartupTime, GloableStaticInstanse.StopWatch.ElapsedMilliseconds);
            ShellShownEvent.SendWith((ShellViewModel)ViewModel);
        }
        #region Fields
        private readonly WindowLogic _windowLogic;

        //IEventAggregator eventAggregator;
        #endregion
      public ShellRibbon Ribbon { get; internal set; }
      
      protected  void CustomiseMainWindowIcon(string applicationIconLocation)
      {
          var directory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
          try
          {
              if (directory != null)
              {
                  string firstAttemptFile = Path.Combine(directory, applicationIconLocation);

                  if (File.Exists(firstAttemptFile))
                  {
                      Icon = BitmapFrame.Create(new Uri(firstAttemptFile, UriKind.Absolute));
                  }
              }
          }
          catch
          {
              // Don't change default Icon.            
          }
      }
      #region IView Members
      /// <summary>
      /// Gets the view model that is contained by the container.
      /// </summary>
      public IViewModel ViewModel
      {
          get { return _windowLogic.ViewModel; }
      }
      /// <summary>
      /// Occurs when the view model container is loading.
      /// </summary>
      public event EventHandler<EventArgs> ViewLoading;
      /// <summary>
      /// Occurs when the view model container is loaded.
      /// </summary>
      public event EventHandler<EventArgs> ViewLoaded;
      /// <summary>
      /// Occurs when the view model container starts unloading.
      /// </summary>
      public event EventHandler<EventArgs> ViewUnloading;
      /// <summary>
      /// Occurs when the view model container is unloaded.
      /// </summary>
      public event EventHandler<EventArgs> ViewUnloaded;
      /// <summary>
      /// Occurs when the <see cref="ViewModel"/> property has changed.
      /// </summary>
      public event EventHandler<EventArgs> ViewModelChanged;

      /// <summary>
      /// Occurs when a property on the <see cref="ViewModel"/> has changed.
      /// </summary>
      public event EventHandler<PropertyChangedEventArgs> ViewModelPropertyChanged;

      /// <summary>
      /// Occurs when a property on the container has changed.
      /// </summary>
      /// <remarks>
      /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
      /// (mostly the container of a view model) because the .NET Framework does not allows us to.
      /// </remarks>
      public event PropertyChangedEventHandler PropertyChanged;
      #endregion
    }
}
