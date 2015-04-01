using Catel.IoC;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MetaShell.Services
{
    public class SplashScreenservice
    {
        public static void ResisterService()
        {
            var serviceLocator = ServiceLocator.Default;
            //serviceLocator.RegisterInstance<MetaBootstrapper>(this);
            var viewLocator = serviceLocator.ResolveType<IViewLocator>();
            viewLocator.Register(typeof(ProgressNotifyableViewModel), typeof(MetaShell.Views.SplashScreen));
            var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
            viewModelLocator.Register(typeof(MetaShell.Views.SplashScreen), typeof(ProgressNotifyableViewModel));
        }
    }
}
