using Catel.IoC;
using Metaseed.MetaShell.Services;

namespace Metaseed.ShellBase
{
    internal class ServiceRegister
    {
        /// <summary>
        /// called by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
        /// Metaseed.ShellBase.ServiceRegister.Register();
        /// </summary>
        internal static void Register()
        {
            ServiceLocator.Default.RegisterType<IBalloon, TaskBarBalloon>();
            ServiceLocator.Default.RegisterType<IMessager, Messager>();
            ServiceLocator.Default.RegisterType<IMissingAssemblyResolverService, MissingAssemblyResolverService>();
            ServiceLocator.Default.RegisterType<IRibbonService, RibbonService>();
            ServiceLocator.Default.RegisterType<IShellService, ShellService>();
        }
    }
}
