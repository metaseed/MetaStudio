
using System.IO.Packaging;
using Catel.Messaging;

namespace Metaseed.MetaShell.Infrastructure
{
    using ViewModels;
    public class PackageBeforeOpenEvent : MessageBase<PackageBeforeOpenEvent, Package> { }
    public class PackageAfterOpenEvent : MessageBase<PackageAfterOpenEvent, Package> { }
    public class PackageBeforeSaveEvent : MessageBase<PackageBeforeSaveEvent, Package> { }
    public class PackageAfterSaveEvent : MessageBase<PackageAfterSaveEvent, Package> { }
    public class BeforeDelFromPackageEvent : MessageBase<BeforeDelFromPackageEvent, IPackageLayoutContentViewModel> { }
}
