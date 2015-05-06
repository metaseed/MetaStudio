﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandDelegate: RemoteCommandBase
    {
        public RemoteCommandDelegate(IRemoteCommandService commandService, string id,IRemoteCommandServiceCallback callback):base( commandService,  id)
        {
            Callback = callback;
        }
        internal IRemoteCommandServiceCallback Callback;
        RemoteCommandManager_Server commandManager { get { return ((RemoteCommandService_Server)(this.CommandService)).CommandManager; } }
        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            Callback.Excute(this.ID,parameter);
        }
    }
}