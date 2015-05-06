using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metaseed.MVVM.Commands
{
    public interface IRemoteCommand : ICommand
    {
        string ID { get; set; }
        CommandUIData UIData { get; set; }
       
    }
    public struct CommandUIData
    {
        public string Text { get; set; }
        public string IconURL { get; set; }
        public bool IsCheckable { get; set; }
        public bool? IsChecked { get; set; }
    }
}
