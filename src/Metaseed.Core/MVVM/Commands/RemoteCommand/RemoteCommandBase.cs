using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandBase :IRemoteCommand
    {
        private readonly IRemoteCommandService _commandService;

        protected RemoteCommandBase(IRemoteCommandService commandService, string id,string uiType)
        {
            _commandService = commandService;
            ID = id;
            UIType = uiType;
        }

        protected IRemoteCommandService CommandService { get { return _commandService; } }


        internal void RaiseCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged(sender, e);
        }
        virtual public bool CanExecute(object parameter)
        {
            return true;
        }
        virtual public void Execute(object parameter)
        {
            
        }
        public event EventHandler CanExecuteChanged;
        public string ID { get; set; }
        public string UIData { get; set; }


        public string UIType { get;  set; }
    }


}
