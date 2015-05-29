using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommand: RemoteCommandBase
    {
        public RemoteCommand(IRemoteCommandService commandService, string id, string uiType, string uiData)
            : base(commandService, id,uiType)
        {
            UIData = uiData;
            this.CanExecuteChanged += RemoteCommand_CanExecuteChanged;
        }

        void RemoteCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            CommandService.CanExecuteChanged(this.ID);
        }
        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

        }
    }
}
