using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public enum ServiceIDType
    {
        SystemGlobal,
        SingleAppInstance,
        MultiAppInstance
    }
    public class RemoteCommandServiceController
    {
        private readonly IRemoteCommandService _commandServiceSingleton;
        public RemoteCommandServiceController(IRemoteCommandService commandServiceSingleton)
        {
            _commandServiceSingleton = commandServiceSingleton;
        }

        ServiceHost serviceHost;
        public IRemoteCommandService Start(ServiceIDType serviceType = ServiceIDType.SystemGlobal)
        {
            string serviceID;
            switch (serviceType)
            {
                case ServiceIDType.SingleAppInstance:
                    serviceID = System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
                    break;
                case ServiceIDType.MultiAppInstance:
                    serviceID = System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath) + "/" + Guid.NewGuid().ToString();
                    break;
                case ServiceIDType.SystemGlobal:
                default:
                    serviceID = "default";
                    break;
            }
            serviceID = serviceID.Trim();
            RemoteCommandService_Server.ServiceID = serviceID;
            serviceHost = new ServiceHost(_commandServiceSingleton, new Uri[] { new Uri("net.pipe://localhost/" + serviceID + "/IRemoteCommandService") });
            var timeout = TimeSpan.MaxValue;/*new TimeSpan(0,10,0)*/
            serviceHost.AddServiceEndpoint(typeof(IRemoteCommandService), new NetNamedPipeBinding()
            {
                SendTimeout = timeout,
                ReceiveTimeout = timeout,
                OpenTimeout = timeout,
                CloseTimeout = timeout
            }, "");
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

        internal static string ServiceID;

        internal RemoteCommandManager_Server CommandManager;
        protected internal IRemoteCommandUIBuilder UIBuilder { get; set; }


        public RemoteCommandService_Server()
        {
            CommandManager = new RemoteCommandManager_Server(this);
        }


        #region IRemoteCommandService
        public virtual void Register(string commandID, string uiType, string uiData)
        {
            CommandManager.Add(commandID, uiType, uiData);

        }

        public virtual void UnRegister(string commandID)
        {
            CommandManager.Remove(commandID);
        }

        public virtual void CanExecuteChanged(string commandID)
        {
            CommandManager[commandID]?.RaiseCanExecuteChanged(this, null);
        }
        #endregion IRemoteCommandService
    }
}
