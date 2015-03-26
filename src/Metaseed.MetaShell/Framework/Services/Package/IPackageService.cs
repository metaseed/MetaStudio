using Catel.MVVM;
using Metaseed.MetaShell.ViewModels;

namespace Metaseed.MetaShell.Services
{
    public interface IPackageService
    {
        string CurrentPackagePath { get; set; }
        Command<IPackageContent, IPackageContent> OpenDocumentCommand { get; }
        void Open(string fileName);
        bool SavePackageContent(IPackageLayoutContentViewModel layoutContentViewModel);
        void Save(string fileName);
    }
}
