using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public interface IRemoteCommandUIBuilder
    {
        void GenerateUI(RemoteCommandDelegate command);
    }
}
