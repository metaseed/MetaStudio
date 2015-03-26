
using System.IO;

namespace Metaseed.MetaShell.ViewModels
{
    public interface IPackageContent
    {
        string PackagePartID { get; }
        string PackagePartName { get; }
        string PackagePartType { get; }
    }
}
