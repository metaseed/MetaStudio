using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandServiceController
    {
        private readonly IRemoteCommandService _commandServiceSingleton;
        public RemoteCommandServiceController(IRemoteCommandService commandServiceSingleton)
        {
            _commandServiceSingleton = commandServiceSingleton;
        }

        ServiceHost serviceHost;
        public IRemoteCommandService Start(string serviceID="")
        {
            serviceHost = new ServiceHost(_commandServiceSingleton, new Uri[] { new Uri("net.pipe://localhost/") });
            var timeout = TimeSpan.MaxValue;/*new TimeSpan(0,10,0)*/
            serviceHost.AddServiceEndpoint(typeof(IRemoteCommandService), new NetNamedPipeBinding()
            {
                SendTimeout = timeout,
                ReceiveTimeout = timeout,
                OpenTimeout = timeout,
                CloseTimeout = timeout
            }, serviceID+"IRemoteCommandService");
            serviceHost.Open();
            foreach (var serviceEndpoint in serviceHost.Description.Endpoints)
            {
                Debug.WriteLine(serviceEndpoint.ListenUri.AbsoluteUri);
            }
            return _commandServiceSingleton;
        }

        public void Stop()
        {
            if (serviceHost != null) serviceHost.Close();
        }
    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,UseSynchronizationContext = false)]
    public class RemoteCommandService_Server : IRemoteCommandService
    {



        internal RemoteCommandManager_Server CommandManager;
        protected internal IRemoteCommandUIBuilder UIBuilder { get; set; }


        public RemoteCommandService_Server()
        {
            CommandManager = new RemoteCommandManager_Server(this);
        }


        #region IRemoteCommandService
        public void Register(string commandID, string uiType, string uiData)
        {
            CommandManager.Add(commandID, uiType, uiData);

        }

        void IRemoteCommandService.UnRegister(string commandID)
        {
            CommandManager.Remove(commandID);
        }

        void IRemoteCommandService.CanExecuteChanged(string commandID)
        {
            CommandManager[commandID].RaiseCanExecuteChanged(this, null);
        }
        #endregion IRemoteCommandService


    }
}
