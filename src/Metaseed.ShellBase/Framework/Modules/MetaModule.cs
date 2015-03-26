using Metaseed.MetaShell.Services;
using Microsoft.Practices.Prism.Modularity;
using Catel.IoC;
using Catel.Logging;
using Fluent;
using Catel.Modules;
namespace Metaseed.Modules
{
    /// <summary>
    /// Base class for all modules
    /// </summary>
    //[Module] has been set on  Catel.Modules.ModuleBase
    public abstract class MetaModule : ModuleBase
    {
        /// <summary>
        /// The modules directory name.
        /// </summary>
        public const string ModulesDirectory = @".\Modules";
        public const string MetaStudioModuleAssemblyFirstCharacter = "☯";
        protected static ILog Log = LogManager.GetCurrentClassLogger();
        
        static  MetaModule()
        {
            ModuleManager =ServiceLocator.Default.ResolveType<IModuleManager>();
            ModuleManager.LoadModuleCompleted += ModuleManager_LoadModuleCompleted;
            ModuleManager.ModuleDownloadProgressChanged += RecordModuleDownloading;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaModule"/> class.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        public MetaModule(string moduleName)
            : base(moduleName)
        {
            Log.Info("Module {0} Constructed.", ModuleName);
        }
        virtual public RibbonContextualTabGroup ContextualTabGroup {
            get { return null; }
        }
        protected string ModuleName_Localized
        {
            get { return ModuleName; }
        }

        protected override void OnInitializing()
        {
            base.OnInitializing();

        }
        protected override void OnRegisterViewsAndTypes()
        {
            base.OnRegisterViewsAndTypes();

        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Log.Info("Module {0} Initialized", ModuleName);
        }
       static IShellService _shellService;
       static protected IShellService ShellService
        {
            get { return _shellService ?? (_shellService = ServiceLocator.Default.ResolveType<IShellService>()); }
        }


        #region module downloading logging
        // The shell imports IModuleManager once to load modules on-demand.
        protected static IModuleManager ModuleManager;

        static void RecordModuleDownloading(object sender, ModuleDownloadProgressChangedEventArgs e)
        {
            string msg = string.Format("'{0}' module is loading {1}/{2}={3} bytes.", e.ModuleInfo.ModuleName, e.BytesReceived, e.BytesReceived, e.ProgressPercentage);
            Log.Info(msg);
        }

        static private void ModuleManager_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            string msg = string.Format(string.Format("'{0}' module loaded.", e.ModuleInfo.ModuleName));
            Log.Info(msg);
        }
        #endregion
    }
}
