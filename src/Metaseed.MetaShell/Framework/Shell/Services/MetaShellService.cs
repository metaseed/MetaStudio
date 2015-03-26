using Catel.IoC;
using System.IO.Packaging;
using System.IO;

namespace Metaseed.MetaShell.Services
{
    using ViewModels;
    using Infrastructure;

    public class MetaShellService : ShellService
    {
        private IPackageService _packageService;
        public IPackageService PackageService
        {
            get
            {
                if (_packageService != null) return _packageService;
                _packageService = this.GetDependencyResolver().Resolve<IPackageService>();
                return _packageService;
            }
        }

        public override void DeleteDocument(ILayoutContentViewModel documentViewModel)
        {
            base.DeleteDocument(documentViewModel);
            if (string.IsNullOrEmpty(PackageService.CurrentPackagePath))
            {
                return;
            }
            string packagePartId = "";
            Services.PackageService.ContentID_ToTypeAndPackagePartID(documentViewModel.ContentId, out packagePartId);
            using (Package package = Package.Open(PackageService.CurrentPackagePath, FileMode.Open))
            {
                Services.PackageService.SaveDocumentsUnopen(package, DocumentsUnopen);
                Services.PackageService.SavePanelLayout(package);
                var exist = Services.PackageService.IsContentExist(package, packagePartId);
                if (!exist)
                {
                    return;
                }
                var contentId = documentViewModel.ContentId;
                BeforeDelFromPackageEvent.SendWith(documentViewModel as IPackageLayoutContentViewModel);
                Services.PackageService.DeletePackageContent(package, contentId);
            }
        }
    }
}
