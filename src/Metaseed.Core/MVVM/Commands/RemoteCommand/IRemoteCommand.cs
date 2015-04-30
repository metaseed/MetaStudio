using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metaseed.MVVM.Commands
{
    public interface IRemoteCommand:ICommand
    {
        string ID { get; set; }
        string Text { get; set; }
        string IconURL { get; set; }
    }
}
