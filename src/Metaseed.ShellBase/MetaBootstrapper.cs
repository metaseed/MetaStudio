using System;
using System.Windows;
using System.Reflection;
using System.Linq;
using System.IO;
using Metaseed.MVVM.Commands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Modules;
using System.Xml.Serialization;
namespace Metaseed.MetaShell
{
    using Modules;
    using Services;
    /// <summary>
    /// creat and run the shell
    /// </summary>
    public  class MetaBootstrapper<TShellView> : BootstrapperBase<TShellView> where TShellView : System.Windows.Window
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private RibbonRemoteCommandServer remoteCommandServiceServer;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly CallbackLogger _callbackLogger = new CallbackLogger();
        #region Constructors
        public MetaBootstrapper(IRemoteCommandUIBuilder uiBuilder=null, ServiceIDType remoteCommandServiceID = ServiceIDType.SystemGlobal)
        {
#if DEBUG
            Catel.Logging.LogManager.AddDebugListener(false);//note: could called multimes. if debug listener already registed,it do nothing.
#endif
            remoteCommandServiceServer = new RibbonRemoteCommandServer(uiBuilder);
            var serviceController = new RemoteCommandServiceController(remoteCommandServiceServer);
            serviceController.Start(remoteCommandServiceID);
            //
            // Application Themes
            //
            Log.Debug("Loading Application  Themes");
            //var ribbonType = typeof(Fluent.Ribbon);
            //Log.Debug("Loaded ribbon type '{0}'", ribbonType.Name);
            var application = Application.Current;
            application.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
            //application.Resources.MergedDictionaries.Add(new ResourceDictionary
            //{
            //    Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            //});
            //application.Resources.MergedDictionaries.Add(new ResourceDictionary
            //{
            //    Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Office2010/Silver.xaml", UriKind.RelativeOrAbsolute)
            //});
            //application.Resources.MergedDictionaries.Add(new ResourceDictionary
            //{
            //    Source = new Uri("pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.Aero;component/Theme.xaml", UriKind.RelativeOrAbsolute)
            //});
            application.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Metaseed.ShellBase;component/Themes/Generic/generic.xaml", UriKind.RelativeOrAbsolute)
            });
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            string modulesDirectory = ModulesDirectory;
            if (!Directory.Exists(modulesDirectory))
            {
                Log.Warning("Modules path '{0}' is missing, new one created", modulesDirectory);
                Directory.CreateDirectory(modulesDirectory);
            }

            CreatedShell += (sender, e) =>
            {
                //var statusBarService = ServiceLocator.Default.ResolveType<IStatusBarService>();
                //statusBarService.UpdateStatus("Ready");
            };

        }
        #endregion

        #region Properties

        private string ModulesDirectory { get { return Path.Combine(Metaseed.AppEnvironment.AppPath, Services.MissingAssemblyResolverService.ModulesDirectory); } }
        #endregion

        #region Methods

        public CompositeModuleCatalog CompositeModuleCatalog;
        /// <summary>
        /// Creates the <see cref="T:Microsoft.Practices.Prism.Modularity.IModuleCatalog"/> used by Prism.
        /// </summary>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var modulesDefFile = Path.Combine(Metaseed.AppEnvironment.AppPath, @"Modules\ModulesDef.xml");
            var compositeModuleCatalog = new CompositeModuleCatalog();
            // compositeModuleCatalog.Add(new NuGetBasedModuleCatalog());
            var serializer = new XmlSerializer(typeof(ModuleInfo[]));
            var forceScanModules_DebugHelper = true;//debug only
            var exception = false;
            var modulesDefFileExist=File.Exists(modulesDefFile);
            if (!forceScanModules_DebugHelper&&modulesDefFileExist)
            {         
                try
                {
                    using (FileStream s = new FileStream(modulesDefFile, FileMode.Open))
                    {
                        var m = serializer.Deserialize(s);
                        var modules = m as ModuleInfo[];
                        foreach (ModuleInfo module in modules)
                        {
                            compositeModuleCatalog.AddModule(module);
                        }
                    }
                }
                catch (Exception)
                {
                    exception = true;
                }
            }
            if (forceScanModules_DebugHelper || (!modulesDefFileExist) || exception)
            {    
                var allSubDirectoriesModuleCatalog = new MetaStudioModuleCatalog { ModulePath = ModulesDirectory };//AllSubDirectoriesModuleCatalog MetaStudioModuleCatalog
                compositeModuleCatalog.Add(allSubDirectoriesModuleCatalog);
                allSubDirectoriesModuleCatalog.Initialize();
                if (!forceScanModules_DebugHelper)
                {
                    using (FileStream s = new FileStream(modulesDefFile, FileMode.Create))
                    {
                        var modules = allSubDirectoriesModuleCatalog.Modules.ToArray();
                        serializer.Serialize(s, modules);//System.Windows.Markup.XamlWriter
                    }
                    Log.Info("Rebuild modules def file");
                }
            }
            Type shellModuleType = typeof(ShellModule);
            compositeModuleCatalog.AddModule(
              new ModuleInfo()
              {
                  ModuleName = shellModuleType.Name,
                  ModuleType = shellModuleType.AssemblyQualifiedName,
              });

            return CompositeModuleCatalog=compositeModuleCatalog;
        }


        /// <summary>
        /// Configures the container.
        /// </summary>
        protected override void ConfigureContainer()
        {

            base.ConfigureContainer();
            LogManager.AddListener(_callbackLogger);
            Container.RegisterInstance(_callbackLogger);
            Container.MissingType += Container_MissingType;
            //Container.RegisterType<IOrchestraService, OrchestraService>();
            //Container.RegisterType<IStatusBarService, StatusBarService>();
            //Container.RegisterType<IRibbonService, RibbonService>();
            //Container.RegisterInstance<IConfigurationService>(new ConfigurationService());

            //add SelectorRegionAdapter,ContentControlRegionAdapter,ItemsControlRegionAdapter here,
            //otherwise when call base.ConfigureRegionAdapterMappings(); type missing error will occor
            //and we could using RegisterType<TService, TServiceImplementation> to overide the default selectors
            Container.RegisterType<SelectorRegionAdapter>();
            Container.RegisterType<ItemsControlRegionAdapter>();
            Container.RegisterType<ContentControlRegionAdapter>();

            //
            Container.RegisterType<IAuthenticationProvider, Authentication>();
        }

        void Container_MissingType(object sender, MissingTypeEventArgs e)
        {
            Log.Warning("Ioc Container Missing Type " + e.InterfaceType.FullName);
        }


        //config default region adapter mappings
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();
            //mappings.RegisterMapping(typeof(Fluent.Ribbon), Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<FluentRibbonRegionAdapter>());
            //var regionBehaviorFactory = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<RegionBehaviorFactory>();
            //mappings.RegisterMapping(typeof(Fluent.Ribbon), new FluentRibbonRegionAdapter(regionBehaviorFactory));
            //mappings.RegisterMapping(typeof(LayoutAnchorablePane), new AvalonDockLayoutAnchorablePaneRegionAdapter(regionBehaviorFactory));
            //mappings.RegisterMapping(typeof(DockingManager), new AvalonDockDockingManagerRegionAdapter(regionBehaviorFactory));
            return mappings;
        }
        //Configure Default Region Behaviors
        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            //factory.AddIfMissing("AutoPopulateExportedViewsBehavior", typeof(AutoPopulateExportedViewsBehavior));
            return factory;
        }
        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog.
        /// </summary>
        protected override void InitializeModules()
        {
            base.InitializeModules();

            //var configurationService = (IConfigurationService)ServiceLocator.Default.GetService(typeof(IConfigurationService));
            //if (_createAboutRibbon)
            //{
            //    var ribbonService = Container.ResolveType<IRibbonService>();
            //    ribbonService.RegisterRibbonItem(new RibbonButton(configurationService.Configuration.HelpTabText, configurationService.Configuration.HelpGroupText, configurationService.Configuration.HelpButtonText, new Command(() =>
            //    {
            //        var uiVisualizerService = Container.ResolveType<IUIVisualizerService>();
            //        uiVisualizerService.ShowDialog(new AboutViewModel());
            //    })));
            //}
        }
        //https://catelproject.atlassian.net/browse/CTL-273
        //Remove external container registration
        //public static void UsingMefContainer()
        //{
        //    AggregateCatalog aggregateCatalog = new AggregateCatalog();
        //    //Add this assembly to export other exported class
        //    //Metaseed.MetaShell.dll
        //    aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MetaBootstrapper).Assembly));
        //    //Metaseed.MetaCore.dll
        //    aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Metaseed.MetaCore.MetaCore).Assembly));
        //    // Config modules referenced directly in the codes of this project.
        //    // Config modules copied to a directory as part of a post-build step.
        //    // These modules are not referenced in the project and are discovered by inspecting a directory.
        //    // These module projects could use a post-build step to copy themselves into that directory
        //    //i.e.:
        //    //xcopy "$(TargetDir)ModularityWithMef.Desktop.ModuleB.dll" "$(SolutionDir)ModularityWithMef.Desktop\bin\$(ConfigurationName)\DirectoryModules\" /Y
        //    //http://compositewpf.codeplex.com/workitem/8324 include all subdirectories
        //    //DirectoryCatalog catalog = new DirectoryCatalog(MetaModule.ModulesDirectory);
        //    //string[] folders = System.IO.Directory.GetDirectories(MetaModule.ModulesDirectory, "*", System.IO.SearchOption.AllDirectories);
        //    //foreach (string folder in folders)
        //    //{please use MetaStudioModuleCatalog
        //    //    DirectoryCatalog catalog = new DirectoryCatalog(folder);
        //    //    aggregateCatalog.Catalogs.Add(catalog);
        //    //}
        //    var mefContainer = new CompositionContainer(aggregateCatalog, new ExportProvider[0]);
        //    ServiceLocator.Default.RegisterExternalContainer(mefContainer);
        //}


        IMissingAssemblyResolverService missingAssemblyResolverService;

        IMissingAssemblyResolverService MissingAssemblyResolverService
        {
            get
            {
                if (missingAssemblyResolverService != null) return missingAssemblyResolverService;
                var dependencyResolver = this.GetDependencyResolver();
                missingAssemblyResolverService = dependencyResolver.Resolve<IMissingAssemblyResolverService>();
                return missingAssemblyResolverService;
            }
        }
        /// <summary>
        /// Called when the resolving of assemblies failed. In that case, this method will try to load the 
        /// assemblies from the modules directory.
        /// </summary>
        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (this)
            {
                //http://stackoverflow.com/questions/4368201/appdomain-currentdomain-assemblyresolve-asking-for-a-appname-resources-assembl
                // failing to ignore queries for satellite resource assemblies or using [assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)] 
                // in AssemblyInfo.cs will crash the program on non en-US based system cultures.
                var fields = args.Name.Split(',');
                var assemblyName = fields[0];
                var fieldsCount = fields.Count();
                if (fieldsCount > 3)
                {
                    var culture = fields[2];
                    if (assemblyName.EndsWith(".resources") && !culture.EndsWith("neutral")) return null;

                    var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in domainAssemblies)
                    {
                        if (string.CompareOrdinal(assembly.GetName().FullName, args.Name) != 0) continue;
                        Log.Info("Loading assembly '{0}' is not required because it is already loaded", args.Name);
                        return assembly;
                    }
                }
                else if (fieldsCount == 1)
                {
                    var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in domainAssemblies)
                    {
                        if (string.CompareOrdinal(assembly.GetName().Name, args.Name) != 0) continue;
                        Log.Info("Loading assembly '{0}' is not required because it is already loaded", args.Name);
                        return assembly;
                    }
                }
                else
                {
                    throw new Exception("Unexpected Assembly Name:" + args.Name);
                }

                var assembly1 = MissingAssemblyResolverService.ResolveAssembly(assemblyName);
                if (assembly1 != null)
                {
                    return assembly1;
                }
                Log.Error("Failed to delay resolve assembly '{0}'", args.Name);
                return null;
            }
        }
        #endregion
    }
}
