using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RemoteCommandManager_Server
    {
        private readonly RemoteCommandService_Server _service;
        public RemoteCommandManager_Server(RemoteCommandService_Server service)
        {
            _service = service;
        }

        internal List<RemoteCommandDelegate> GetCommands(string commandID)
        {
            if (Commands.ContainsKey(commandID))
            {
                return Commands[commandID].CommandDelegates.Values.ToList();
            }
            return null;
        }
        internal RemoteCommandDelegate this[string commandID]
        {
            get
            {
                return Commands[commandID].CommandDelegates[Callback];
            }
        }
        internal readonly Dictionary<string, CompositeRemoteCommand> Commands = new Dictionary<string, CompositeRemoteCommand>();

        internal void Add(string commandID, string uiData)
        {
            var callback = Callback;
            if (Commands.ContainsKey(commandID))
            {
                var compositeRemoteCommand = Commands[commandID];
                compositeRemoteCommand.Add(commandID, uiData);
            }
            else
            {
                var compositeRemoteCommand = new CompositeRemoteCommand(_service, commandID){UIData = uiData};
                this.Commands.Add(commandID,compositeRemoteCommand);
                compositeRemoteCommand.Add(commandID,uiData);
                if (_service.UIBuilder != null) _service.UIBuilder.GenerateUI(compositeRemoteCommand);
            }
        }

        internal void Remove(string commandID)
        {
            if (Commands.ContainsKey(commandID))
            {
                var callback = Callback;
                var compositeRemoteCommand = Commands[commandID];

                if (compositeRemoteCommand.CommandDelegates.Count == 0)
                {
                    Commands.Remove(commandID);
                    if (_service.UIBuilder != null) _service.UIBuilder.RemoveUI(commandID);
                }
            }
            
        }

        IRemoteCommandServiceCallback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IRemoteCommandServiceCallback>();
            }
        }
    }
}
