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
        private bool passCanExcuteCall = true;//when binding we pass it
        public CompositeRemoteCommand(IRemoteCommandService commandService, string id)
            : base(commandService, id)
        {

        }
        public RibbonRemoteCommandUIData DeserializedUIData;
        internal Dictionary<IRemoteCommandServiceCallback, RemoteCommandDelegate> CommandDelegates =
            new Dictionary<IRemoteCommandServiceCallback, RemoteCommandDelegate>();
        public override bool CanExecute(object parameter)
        {
            if (passCanExcuteCall)
            {
                passCanExcuteCall = false;
                return true;
            }
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
        internal RemoteCommandDelegate Add(string commandID, string uiData)
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
                CommandDelegates.Remove(callback);
            }
        }
    }
}
