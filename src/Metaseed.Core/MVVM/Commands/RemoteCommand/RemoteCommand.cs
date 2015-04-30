using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommand :IRemoteCommand
    {
        private readonly IRemoteCommandManager _commandManager;

        public RemoteCommand(IRemoteCommandManager commandManager,string id)
        {
            _commandManager = commandManager;
            ID = id;
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;
        public string ID { get; set; }
        public string Text { get; set; }
        public string IconURL { get; set; }
    }


}
