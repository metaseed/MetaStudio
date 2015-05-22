using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandService:DuplexClientBase<IRemoteCommandService>, IRemoteCommandService
    {
        internal RemoteCommandManager commandManager;

        public RemoteCommandService()
            : this(new RemoteCommandServiceCallback())
        {

        }
        public RemoteCommandService(RemoteCommandServiceCallback callback)
            : base(new InstanceContext(callback), new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteCommandService)),
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/IRemoteCommandService")))
        {
            commandManager = new RemoteCommandManager(this);
            callback.RemoteCommandService = this;
            
        }


        void IRemoteCommandService.Register(string commandID, string uiData)
        {
            Register(new RemoteCommand(this, commandID, uiData));
        }

        public void Register(IRemoteCommand command)
        {
            commandManager.Add(command);
            Channel.Register(command.ID,command.UIType, command.UIData);
            
        }

        void IRemoteCommandService.UnRegister(string commandID)
        {
            commandManager.Remove(commandID);
            Channel.UnRegister(commandID);
        }

         void IRemoteCommandService. CanExecuteChanged(string commandID)
        {
            Channel.CanExecuteChanged(commandID);
        }
    }


}
