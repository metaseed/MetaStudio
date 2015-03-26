using System.Threading.Tasks;
using Catel.IoC;
using Metaseed.MetaShell.Services;

namespace Metaseed.MetaShell.ViewModels
{
    public class ToolViewModel : ToolBaseViewModel, IPackageLayoutContentViewModel
    {
        protected override async Task<bool> Save()
        {
            base.Save();
            return PackageService.SavePackageContent(this);
        }
        /// <summary>
        /// saved in avlondock panel layout
        /// used to load and creat the content from package
        /// </summary>
        public override string ContentId
        {
            get { return Services.PackageService.ContentId(this); }
            set
            {
                Services.PackageService.SetContentViewModelID(value, this);
            }
        }


        public string PackagePartID
        {
            get { return Services.PackageService.PackagePartID(this); }
        }

        public string PackagePartName
        {
            get { return Services.PackageService.PackagePartName(this); }
        }

        public string PackagePartType
        {
            get { return Services.PackageService.PackagePartType(this); }
        }
        private IPackageService _packageService;
        public IPackageService PackageService
        {
            get
            {
                if (_packageService != null) return _packageService;
                _packageService = DependencyResolver.Resolve<IPackageService>();
                return _packageService;
            }
        }

    }
}
