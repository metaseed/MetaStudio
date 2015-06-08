using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        internal void Add(string commandID,string uiType, string uiData)
        {
            var callback = Callback;
            if (Commands.ContainsKey(commandID))
            {
                var compositeRemoteCommand = Commands[commandID];
                compositeRemoteCommand.Add(commandID,uiType, uiData);
            }
            else
            {
                var compositeRemoteCommand = new CompositeRemoteCommand(_service, commandID,uiType){UIData = uiData};
                this.Commands.Add(commandID,compositeRemoteCommand);
                compositeRemoteCommand.Add(commandID,uiType,uiData);

                if (_service.UIBuilder != null)
                {
                    if (Application.Current!=null&&Application.Current.Dispatcher != null&&!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            (Action) (() => _service.UIBuilder.GenerateUI(compositeRemoteCommand)));
                    }
                    else
                    {
                        _service.UIBuilder.GenerateUI(compositeRemoteCommand);
                    }
                }
            }
        }

        internal void Remove(string commandID)
        {
            if (Commands.ContainsKey(commandID))
            {
                var callback = Callback;
                var compositeRemoteCommand = Commands[commandID];

                if (compositeRemoteCommand.CommandDelegates.Count != 0)
                {
                    Commands.Remove(commandID);
                    if (Application.Current != null && Application.Current.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            (Action)(() => _service.UIBuilder.RemoveUI(compositeRemoteCommand)));
                    }
                    else
                    {
                        _service.UIBuilder.RemoveUI(compositeRemoteCommand);
                    }
                    
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
