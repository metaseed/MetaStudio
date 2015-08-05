using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandService:DuplexClientBase<IRemoteCommandService>, IRemoteCommandService
    {
        internal RemoteCommandManager commandManager;

        public RemoteCommandService(ServiceIDType serviceIDType= ServiceIDType.SystemGlobal)
            : this(serviceIDType,new RemoteCommandServiceCallback())
        {

        }
        static string GenerateServiceID(ServiceIDType serviceIDType)
        {
            var args= Environment.GetCommandLineArgs();
            var pre = "-remoteCommandServiceID=";
            foreach (var arg in args)
            {
                if (arg.Contains(pre))
                {
                    var start = arg.IndexOf(pre) + pre.Length;
                    var commandServiceID = arg.Substring(start);
                    return commandServiceID;
                }
            }
            if (serviceIDType != ServiceIDType.SystemGlobal)
            {
                var processName=System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
                MessageBox.Show("Could not find -remoteCommandServiceID process argument in process: "+ processName, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
            return "default";
        }
        public RemoteCommandService(ServiceIDType serviceIDType,RemoteCommandServiceCallback callback)
            : base(new InstanceContext(callback), new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteCommandService)),
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/" + GenerateServiceID(serviceIDType) + "/IRemoteCommandService")))
        {
            commandManager = new RemoteCommandManager(this);
            callback.RemoteCommandService = this;
            var timeout = TimeSpan.MaxValue;//new TimeSpan(0,10,0);
            this.Endpoint.Binding.SendTimeout = timeout;
            this.Endpoint.Binding.ReceiveTimeout = timeout;
            this.Endpoint.Binding.OpenTimeout = timeout;
            this.Endpoint.Binding.ReceiveTimeout = timeout;
        }


        void IRemoteCommandService.Register(string commandID, string uiType, string uiData)
        {
            Register(new RemoteCommand(this, commandID,uiType, uiData));
        }

        public void Register(IRemoteCommand command)
        {
            if (this.State != CommunicationState.Opened)
            {
                MessageBox.Show("Can not regisger command, Remote Command Communication Error!");
                return;
            }
            commandManager.Add(command);
            Channel.Register(command.ID, command.UIType, command.UIData);
            //ThreadPool.QueueUserWorkItem((o)=>Channel.Register(command.ID, command.UIType, command.UIData));
          
        }

        public void UnRegister(string commandID)
        {
            if (this.State != CommunicationState.Opened)
            {
                MessageBox.Show("Can not unregister command, Remote Command Communication Error!");
                return;
            }
            commandManager.Remove(commandID);
            Channel.UnRegister(commandID);
        }

        void IRemoteCommandService.UnRegister(string commandID)
        {
            UnRegister(commandID);
        }

         void IRemoteCommandService.CanExecuteChanged(string commandID)
        {
            if (this.State != CommunicationState.Opened)
            {
                MessageBox.Show("Can not call CanExecuteChanged, Remote Command Communication Error!");
                return;
            }
            Channel.CanExecuteChanged(commandID);
        }
    }


}
