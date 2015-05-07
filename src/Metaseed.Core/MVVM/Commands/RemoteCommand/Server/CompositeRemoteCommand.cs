using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class CompositeRemoteCommand:RemoteCommandBase
    {
        public CompositeRemoteCommand(IRemoteCommandService commandService, string id)
            : base(commandService, id)
        {
        }
        internal Dictionary<IRemoteCommandServiceCallback, RemoteCommandDelegate> CommandDelegates =
            new Dictionary<IRemoteCommandServiceCallback, RemoteCommandDelegate>();
        public override bool CanExecute(object parameter)
        {
            return CommandDelegates.Select(remoteCommandPair => remoteCommandPair.Value.CanExecute(parameter)).Any(canExcu => canExcu);
        }


        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            foreach (var remoteCommandPair in CommandDelegates)
            {
                remoteCommandPair.Value.Execute(parameter);
            }
        }
        IRemoteCommandServiceCallback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IRemoteCommandServiceCallback>();
            }
        }
        internal RemoteCommandDelegate Add(string commandID, CommandUIData uiData)
        {
            var callback = Callback;
            if (!CommandDelegates.ContainsKey(callback))
            {
                var command = new RemoteCommandDelegate(CommandService, commandID, callback) { UIData = uiData };
                command.CanExecuteChanged += RaiseCanExecuteChanged;
                CommandDelegates.Add(callback, command);
                return command;
            }
            else
            {
                var fault = new ValidationFault
                {
                    Result = false,
                    Message = "The Remote Command Of This Client With Same ID Has Already Been Added!",
                    Description = "Invalid CommandID"
                };
                throw new FaultException<ValidationFault>(fault);
            }
        }



        internal void Remove(string commandID)
        {
            var callback = Callback;
            if (CommandDelegates.ContainsKey(callback))
            {
                CommandDelegates.Remove(callback);
            }
        }
    }
}
