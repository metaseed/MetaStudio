using Catel.IoC;
using Metaseed.MetaShell.Services;

namespace Metaseed.ShellBase
{
    internal class ServiceRegister
    {
        /// <summary>
        /// Metaseed.ShellBase.ServiceRegister.Register();
        /// </summary>
        internal static void Register()
        {
            ServiceLocator.Default.RegisterType<IBalloon, TaskBarBalloon>();
            ServiceLocator.Default.RegisterType<IMessager, Messager>();
            ServiceLocator.Default.RegisterType<IMissingAssemblyResolverService, MissingAssemblyResolverService>();
            ServiceLocator.Default.RegisterType<IShellService, ShellService>();
        }
    }
}
