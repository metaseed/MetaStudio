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

        private RemoteCommandService_Server _service;
        public RemoteCommandManager_Server(RemoteCommandService_Server service)
        {
            _service = service;
        }


        internal List<IRemoteCommand> GetCommands(string commandID)
        {
            if (Commands.ContainsKey(commandID))
            {
                return Commands[commandID].Values.ToList();
            }
            return null;
        }

        internal readonly Dictionary<string, Dictionary<IRemoteCommandServiceCallback, IRemoteCommand>> Commands = new Dictionary<string, Dictionary<IRemoteCommandServiceCallback, IRemoteCommand>>();

        internal void Add(string commandID, CommandUIData uiData)
        {
            if (Commands.ContainsKey(commandID))
            {
                var clientCommandDic = Commands[commandID];
                if (!clientCommandDic.ContainsKey(Callback))
                {
                    var command = new RemoteCommandDelegate(_service, commandID, Callback) { UIData = uiData };
                    clientCommandDic.Add(Callback, command);
                    if (_service.UIBuilder != null) _service.UIBuilder.GenerateUI(command);
                }
                else
                {
                    var fault = new ValidationFault
                     {
                         Result = false,
                         Message = "Numbers cannot be zero",
                         Description = "Invalid numbers"
                     };
                    throw new FaultException<ValidationFault>(fault);
                }
            }
            else
            {
                var clientCommandDic = new Dictionary<IRemoteCommandServiceCallback, IRemoteCommand>();
                Commands.Add(commandID, clientCommandDic);
                var command = new RemoteCommandDelegate(_service, commandID, Callback) { UIData = uiData };
                clientCommandDic.Add(Callback, command);
                if(_service.UIBuilder!=null)_service.UIBuilder.GenerateUI(command);
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
