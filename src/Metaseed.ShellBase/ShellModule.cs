using Fluent;
using System.Windows.Media;
using Metaseed.MVVM.Commands;

namespace Metaseed.Modules
{
    using MetaShell.Services;
    using MetaShell.Infrastructure;
    using MetaShell.ViewModels;

    public class ShellModule : MetaModule
    {
        private RibbonRemoteCommandServer remoteCommandServiceServer;
        public ShellModule()
            : base("ShellModule")
        {
            remoteCommandServiceServer = new RibbonRemoteCommandServer();
            var serviceController = new RemoteCommandServiceController(remoteCommandServiceServer);
            serviceController.Start();
        }
        public ShellModule(string moduleName) : base(moduleName) { }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (GloableStaticInstanse.StartupInputFilePathName == null)
            {
                // This workaround is necessary until https://avalondock.codeplex.com/workitem/15577
                // is applied, or the bug is fixed in another way.
                ShellService.Tools.Add(new ToolBaseViewModel(ToolPaneLocation.Bottom) { IsVisible = false });
                ShellService.Tools.Add(new ToolBaseViewModel(ToolPaneLocation.Left) { IsVisible = false });
                ShellService.Tools.Add(new ToolBaseViewModel(ToolPaneLocation.Right) { IsVisible = false });
            }

        }

        RibbonContextualTabGroup _contextualTabGroup;
        public override RibbonContextualTabGroup ContextualTabGroup
        {
            get
            {
                if (_contextualTabGroup == null)
                {
                    _contextualTabGroup = new RibbonContextualTabGroup() { BorderBrush = Brushes.Magenta, Background = Brushes.Purple };
                    LocalizeHelper.BindTo(ContextualTabGroup, RibbonContextualTabGroup.HeaderProperty, "Metaseed.ShellBase", "Resources", "Common");
                    ShellService.Ribbon.AddRibbonContextualTabGroup(ContextualTabGroup);
                }
                return _contextualTabGroup;
            }
        }
    }
}
