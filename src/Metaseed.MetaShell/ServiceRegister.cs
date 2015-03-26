using Catel.IoC;
using Metaseed.MetaShell.Services;

namespace Metaseed.MetaShell
{
    internal class ServiceRegister
    {
        /// <summary>
        /// Metaseed.ShellBase.ServiceRegister.Register();
        /// </summary>
        internal static void Register()
        {
            ServiceLocator.Default.RegisterType<IPackageService, PackageService>();
        }
    }
}
