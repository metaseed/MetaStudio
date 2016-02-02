using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Metaseed.MVVM.Commands
{
    public class CompositeRemoteCommand:RemoteCommandBase
    {
        private bool passCanExcuteCall = false;// we pass the command binding call at the first time, happened when binding applied 
        public CompositeRemoteCommand(IRemoteCommandService commandService, string id,string uiType)
            : base(commandService, id, uiType)
        {

        }
        public object DeserializedUIData;
        internal Dictionary<IRemoteCommandServiceCallback, RemoteCommandDelegate> CommandDelegates =
            new Dictionary<IRemoteCommandServiceCallback, RemoteCommandDelegate>();
        public override bool CanExecute(object parameter)
        {
            if (passCanExcuteCall)
            {
                passCanExcuteCall = false;
                return true;
            }
            return CommandDelegates.Values.ToList().Any(remoteCommandDelegate => remoteCommandDelegate.CanExecute(parameter));
            //return CommandDelegates.Select(remoteCommandPair => remoteCommandPair.Value.CanExecute(parameter)).Any(canExcu => canExcu);
        }


        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            foreach (var remoteCommand in CommandDelegates.Values.ToList())
            {
                remoteCommand.Execute(parameter);
            }
        }
        IRemoteCommandServiceCallback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IRemoteCommandServiceCallback>();
            }
        }

        void CompositeRaiseCanExecuteChanged(object sender, EventArgs e)
        {
            if (Application.Current != null && Application.Current.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => this.RaiseCanExecuteChanged(this,e)));
            }
            else
            {
                this.RaiseCanExecuteChanged(this, e);
            }
        }

        internal RemoteCommandDelegate Add(string commandID, string uiType, string uiData)
        {
            var callback = Callback;
            if (!CommandDelegates.ContainsKey(callback))
            {
                var command = new RemoteCommandDelegate(CommandService, commandID,uiType, callback) { UIData = uiData };
                command.CanExecuteChanged += CompositeRaiseCanExecuteChanged;
                CommandDelegates.Add(callback, command);
                return command;
            }
            else
            {
                var fault = new RemoteCommandFault
                {
                    Result = false,
                    Message = "The Remote Command Of This Client With Same ID Has Already Been Added!",
                    Description = "Invalid CommandID"
                };
                throw new FaultException<RemoteCommandFault>(fault);
            }
        }

        internal void Remove(string commandID)
        {
            var callback = Callback;
            if (CommandDelegates.ContainsKey(callback))
            {
                CommandDelegates[callback].CanExecuteChanged -= CompositeRaiseCanExecuteChanged;
                CommandDelegates.Remove(callback);
            }
        }
    }
}
