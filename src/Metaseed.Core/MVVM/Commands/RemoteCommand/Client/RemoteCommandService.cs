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
        private RemoteCommandServiceCallback _callback;

        public RemoteCommandService()
            : this(new RemoteCommandServiceCallback())
        {

        }
        public RemoteCommandService(RemoteCommandServiceCallback callback)
            : base(new InstanceContext(callback), new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteCommandService)),
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/IRemoteCommandService")))
        {
            commandManager = new RemoteCommandManager(this);
            _callback = callback;
            _callback.RemoteCommandService = this;
            
        }
        //public RemoteCommandService()
        //    : base(new InstanceContext(new RemoteCommandServiceCallback()), new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteCommandService)),
        //        new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/IRemoteCommandService")))
        //{
        //    commandManager = new RemoteCommandManager(this);
        //    //_callback = callback;
        //   // _callback.RemoteCommandService = this;
        //    //((IRemoteCommandService)this).Connect();
        //}


        void IRemoteCommandService.Register(string commandID, CommandUIData uiData)
        {
            Register(new RemoteCommand(this, commandID, uiData));
        }

        public void Register(IRemoteCommand command)
        {
            commandManager.Add(command);
            Channel.Register(command.ID, command.UIData);
            
        }

        public void UnRegister(string commandID)
        {
            Channel.UnRegister(commandID);
        }

        public void CanExecuteChanged(string commandID)
        {
            Channel.CanExecuteChanged(commandID);
        }
    }

    public class RemoteCommandServiceCallback : IRemoteCommandServiceCallback
    {
        internal  RemoteCommandService RemoteCommandService;
        public void Excute(string commandID, object parameter)
        {
            RemoteCommandService.commandManager[commandID].Execute(parameter);
        }

        public bool CanExcute(string commandID, object param)
        {
            return RemoteCommandService.commandManager[commandID].CanExecute(param);
        }
    }
}
