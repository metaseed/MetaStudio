using System;
using System.IO;
using System.Windows.Input;
using Catel.MVVM;
using Microsoft.Practices.Prism;
using System.Windows.Media;
using Catel.Data;
namespace Metaseed.MetaShell.ViewModels
{
    using Data;
    public interface ILayoutContentViewModel : IViewModel,IActiveAware,INameDescription
    {
        /// <summary>
        /// saved in avlondock panel layout, used to load and creat the content from package
        /// </summary>
        string ContentId { get; set; }
        /// <summary>
        /// the default IModel.IsDirty Methord will be true when the RaisePropertyChanged called, it's not what we want.
        /// </summary>
        bool IsDataDirty { get; }
        Guid ID { get;}
        ICommand CloseCommand { get; }
        bool CanClose { get; }
        ImageSource IconSource { get; }
        bool IsSelected { get; set; }
        void SaveState(Stream stream);
        //bool IsManageable { get; }
    }
}