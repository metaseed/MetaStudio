using System;
using System.Windows;
using Catel.IoC;
using Catel.MVVM;
using Metaseed.MetaShell.Infrastructure;
using System.Diagnostics;
using System.Deployment.Application;
using Metaseed.MetaShell.Views;
using Metaseed.Modules;
using Microsoft.Practices.Prism.Modularity;

namespace Metaseed.MetaStudio
{
    using MetaShell;
    using Properties;
    using Metaseed.MetaShell.Services;
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static IExceptionHandler _ExceptionHandler;
        public App()
        {
            GloableStaticInstanse.StopWatch = Stopwatch.StartNew();
            //InterestedAppSettings.RecentFiles = Settings.Default.RecentFiles;
            _ExceptionHandler = new ExceptionHandler();
            TextFileLogger.IsTruncateTheFile = false;
            _ExceptionHandler.AddLogger(new TextFileLogger());
            _ExceptionHandler.AddLogger(new EmailLogger());
            ServiceLocator.Default.RegisterInstance<IExceptionHandler>(_ExceptionHandler);
        }
        private static string GetPath(StartupEventArgs e)
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return e.Args.Length != 0 ? e.Args[0] : null;
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments == null)
                return String.Empty;
            var args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
            return args == null || args.Length == 0 ? String.Empty : new Uri(args[0]).LocalPath;
        }
        private static void InitializeAppCultures()
        {
            //InterestedAppSettings.Culture = Settings.Default.Culture;
            //if (!String.IsNullOrEmpty(Settings.Default.Culture))
            //{
            //    var culture = new CultureInfo(Settings.Default.Culture);
            //    Thread.CurrentThread.CurrentCulture = culture;
            //    Thread.CurrentThread.CurrentUICulture = culture;
            //}
            //// Changes the Default WPF Culture (en-US), otherwise it will be used, instead of the system settings.
            //FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));//CultureInfo.CurrentCulture.IetfLanguageTag is deprecated.
            //note: You can still override individual window when necessary as below
            //this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);
        } 
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            GloableStaticInstanse.StartupInputFilePathName = GetPath(e);
            if (!string.IsNullOrEmpty(GloableStaticInstanse.StartupInputFilePathName))
            {
                GloableStaticInstanse.AppStatus = AppStatus.StaringAndOpeningPackage;
            }
            else
            {
                GloableStaticInstanse.AppStatus = AppStatus.Starting;
            }
            //InitializeAppCultures();
            //
            //Globle Catel Performance Optimizing: 
            //
            //  1.disabling data validating;
            Catel.Windows.Controls.UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;//can be suspended per control
            Catel.Windows.Controls.UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;//can be suspended per control
            Catel.Data.ModelBase.SuspendValidationForAllModels = true;
            // 
            //  2. disabling validation and change notifications;
            //
            //ModelBase.GlobalLeanAndMeanModel = true;//an be suspended per model (using the LeanAndMeanModel property) 
             
            //
            // TO Using a custom IoC container like Unity? Register it here:
            //ServiceLocator.Default.RegisterExternalContainer(unityContainer);
            //MetaBootstrapper.UsingMefContainer();
            Catel.CatelEnvironment.RegisterDefaultViewModelServices();
            //
            //splash screen
            //
            var serviceLocator = ServiceLocator.Default;
            //serviceLocator.RegisterInstance<MetaBootstrapper>(this);
            var viewLocator = serviceLocator.ResolveType<IViewLocator>();
            viewLocator.Register(typeof(ProgressNotifyableViewModel), typeof(MetaShell.Views.SplashScreen));
            var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
            viewModelLocator.Register(typeof(MetaShell.Views.SplashScreen), typeof(ProgressNotifyableViewModel));

            var bootstrapper = new MetaBootstrapper<MetaShellView>();
            bootstrapper.CreatedModuleCatalog+=(sender,e1)=>
            {
                Type metaShellModuleType = typeof(MetaShellModule);

                bootstrapper.CompositeModuleCatalog.AddModule(
                 new ModuleInfo()
                 {
                     ModuleName = metaShellModuleType.Name,
                     ModuleType = metaShellModuleType.AssemblyQualifiedName,
                 });
            };
            bootstrapper.ConfiguredServiceLocatorContainer += (sender, e5) => { };//ConfigureContainer
            bootstrapper.ConfiguredRegionAdapters += (sender, e4) => { };
            bootstrapper.ConfiguredDefaultRegionBehaviors += (sender, e3) => { };
           // StyleHelper.CreateStyleForwardersForDefaultStyles(Current.Resources.MergedDictionaries[0]);//catelTheme
            bootstrapper.CreatedShell += (sender, e2) =>
            {
                //// Configure shell when it's created
                //var configurationService = ServiceLocator.Default.ResolveType<IConfigurationService>();
                //ConfigureShell(configurationService);

                //// Disable debugging window
                //var orchestraService = ServiceLocator.Default.ResolveType<IOrchestraService>();
                //orchestraService.ShowDebuggingWindow = false;
                //ModelBase.GlobalLeanAndMeanModel = true;
            };

#if SPLASH_SCREEN_NO
            bootstrapper.Run();
#else
            bootstrapper.RunWithSplashScreen<ProgressNotifyableViewModel>();
#endif
           // 
            base.OnStartup(e);

        }
        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            base.OnExit(e);
        }
    }
}