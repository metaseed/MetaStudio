using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommand: RemoteCommandBase
    {
        public RemoteCommand(IRemoteCommandService commandService, string id,CommandUIData uiData):base(commandService,id)
        {
            UIData = uiData;
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
