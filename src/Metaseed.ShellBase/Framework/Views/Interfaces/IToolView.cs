using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metaseed.MetaShell.Views
{
    using ViewModels;
    public interface IToolView:ILayoutContentView
    {
        IToolViewModel ToolViewModel { get; }
    }
}
