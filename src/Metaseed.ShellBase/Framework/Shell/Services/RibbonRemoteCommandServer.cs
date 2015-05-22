using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metaseed.MVVM.Commands;
using Metaseed.ShellBase.Framework.Shell.Views;

namespace Metaseed.MetaShell.Services
{
    internal class RibbonRemoteCommandServer : RemoteCommandService_Server
    {
            public RibbonRemoteCommandServer()
        {
            this.UIBuilder = new RibbonRemoteCommandUIBuilder();
        }
    }

}
