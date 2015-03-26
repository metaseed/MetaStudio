using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.MetaShell.Services
{
    using ViewModels;
    public interface IDocumentProvider
    {
        IDocumentViewModel CreatDocument();
        bool CouldNewDocument { get; }
    }
}
