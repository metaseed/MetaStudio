using System.ComponentModel;
using Catel.IoC;
using Metaseed.MetaShell.Services;
using Catel;
using Metaseed.Windows.Interop;

namespace Metaseed.MetaShell.ViewModels
{
    public class MetaShellViewModel : ShellViewModel
    {
        public MetaShellViewModel()
        {
            //PackageBeforeOpenEvent.Register(this, new Action<PackageBeforeOpenEvent>(PackageBeforeOpenEventHandler));
        }

        private IPackageService _packageService;

        public IPackageService PackageService
        {
            get
            {
                if (_packageService != null)
                {
                    return _packageService;
                }
                _packageService = this.GetDependencyResolver().Resolve<IPackageService>();
                this.SubscribeToWeakPropertyChangedEvent(_packageService, PackageService_PropertyChangedEventHandler);
                return _packageService;
            }
        }
        string _truncatedPackagePath;
        public string TruncatedPackagePath
        {
            get { return _truncatedPackagePath; }
            set
            {
                if (value != _truncatedPackagePath)
                {
                    _truncatedPackagePath = value;
                    RaisePropertyChanged(() => this.TruncatedPackagePath);
                }
            }
        }
        void PackageService_PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CurrentPackagePath"))
            {
                TruncatedPackagePath = TruncateFilePath.TruncatePath(_packageService.CurrentPackagePath, 40);
            }
        }



    }
}
