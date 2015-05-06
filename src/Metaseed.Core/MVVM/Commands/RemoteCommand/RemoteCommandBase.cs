using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandBase :IRemoteCommand
    {
        private readonly IRemoteCommandService _commandService;

        protected RemoteCommandBase(IRemoteCommandService commandService, string id)
        {
            _commandService = commandService;
            ID = id;
        }

        protected IRemoteCommandService CommandService { get { return _commandService; } }

        virtual public bool CanExecute(object parameter)
        {
            return true;
        }
        internal void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged(sender, e);
        }
        virtual public void Execute(object parameter)
        {
            
        }
        
        public event EventHandler CanExecuteChanged;
        public string ID { get; set; }
        public CommandUIData UIData { get; set; }
    }


}
