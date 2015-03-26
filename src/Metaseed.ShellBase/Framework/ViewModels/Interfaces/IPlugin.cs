using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluent;
namespace Metaseed.MetaShell.ViewModels
{
    interface IPlugin
    {
        RibbonContextualTabGroup ContextualTabGroup{get;}
    }
}
