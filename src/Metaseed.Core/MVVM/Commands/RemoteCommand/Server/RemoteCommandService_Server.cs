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
        public IRemoteCommandService Start()
          {
              serviceHost = new ServiceHost(_commandServiceSingleton, new Uri[] { new Uri("net.pipe://localhost/") });
             serviceHost.AddServiceEndpoint(typeof(IRemoteCommandService), new NetNamedPipeBinding(), "IRemoteCommandService");
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
     [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RemoteCommandService_Server:IRemoteCommandService
    {
        


        internal  RemoteCommandManager_Server CommandManager;
        protected internal IRemoteCommandUIBuilder UIBuilder { get; set; }


         public RemoteCommandService_Server()
         {
             CommandManager = new RemoteCommandManager_Server(this);
         }

        
        #region IRemoteCommandService
       public  void Register(string commandID, string uiData)
        {
            CommandManager.Add(commandID, uiData);

        }

        void IRemoteCommandService.UnRegister(string commandID)
        {
            
        }

         void IRemoteCommandService.CanExecuteChanged(string commandID)
         {
             CommandManager[commandID].RaiseCanExecuteChanged(this, null);
         }
        #endregion IRemoteCommandService


    }
}
