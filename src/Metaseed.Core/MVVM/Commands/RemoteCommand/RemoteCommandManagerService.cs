using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class RemoteCommandManagerService:IRemoteCommandManager
    {
        private ServiceHost serviceHost;
        public void Start()
        {
            serviceHost = new ServiceHost(typeof(RemoteCommandManagerService), new Uri[] { new Uri("net.pipe://localhost/") });
            serviceHost.AddServiceEndpoint(typeof(IRemoteCommandManager), new NetNamedPipeBinding(), "IRemoteCommandManager");
            serviceHost.Open();
            foreach (var serviceEndpoint in serviceHost.Description.Endpoints)
            {
                Debug.WriteLine(serviceEndpoint.ListenUri.AbsoluteUri);
            }
        }
        internal Dictionary<string,RemoteCommand> Commands=new Dictionary<string, RemoteCommand>();
        #region IRemoteCommandManager
        public void Register(string commandID, string text, string iconURL)
        {
            if (Commands.ContainsKey(commandID))
            {
                var fault = new ValidationFault
                {
                    Result = false,
                    Message = "Numbers cannot be zero",
                    Description = "Invalid numbers"
                };

                throw new FaultException<ValidationFault>(fault);
            }
            Commands.Add(commandID,new RemoteCommand(this,commandID){Text = text,IconURL=iconURL});
            
        }

        public void UnRegister(string commandID)
        {
            throw new NotImplementedException();
        }

        public void CanExecuteChanged(string commandID)
        {
            throw new NotImplementedException();
        }
        #endregion IRemmoteCommandManager
    }
}
