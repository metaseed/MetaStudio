namespace Metaseed.Modules
{
    //using InfoLogger.ViewModels;
    using MetaShell.Infrastructure;
    //using FunctionBlock.Services;
    //using FunctionBlock.ViewModels;

    public class MetaShellModule : ShellModule
    {
        public MetaShellModule()
            : base("MetaShellModule")
        {
            
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (GloableStaticInstanse.StartupInputFilePathName == null)
            {
                //var shellView = ServiceLocator.Default.ResolveType<Metaseed.MetaShell.Views.ShellView>();
                //shellView.Loaded += shellView_Loaded;
                //var boot= ServiceLocator.Default.ResolveType<MetaBootstrapper>();
                //boot.CreatedShell += boot_InitializedModules;

                //ShellService.ShowTool(new InfoLoggerViewModel());

                //_FunctionBlockDocumentViewModel = TypeFactory.Default.CreateInstance<FunctionBlockDesignerDocumentViewModel>();
                //ShellService.OpenDocument(_FunctionBlockDocumentViewModel);
                // ShellService.ActivateDocument(_FunctionBlockDocumentViewModel);

                //var propertyGrid = ServiceLocator.Default.ResolveType<IPropertyGridService>();
                //var functionBlockService = ServiceLocator.Default.ResolveType<IFunctionBlockService>();
                //propertyGrid.SelectedObject = ((FunctionBlockService)functionBlockService)._FunctionBlockDocumentViewModel;

            }

        }

        //void shellView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    ShellService.ActivateDocument(_FunctionBlockDocumentViewModel);
        //}
        //FunctionBlockDesignerDocumentViewModel _FunctionBlockDocumentViewModel;
        //void boot_InitializedModules(object sender, EventArgs e)
        //{
        //    var boot = ServiceLocator.Default.ResolveType<MetaBootstrapper>();
        //    boot.InitializedModules -= boot_InitializedModules;

        //}

    }
}
