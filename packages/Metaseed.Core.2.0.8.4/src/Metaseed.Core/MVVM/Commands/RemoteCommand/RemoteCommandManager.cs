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
    public class RemoteCommandManager:DuplexClientBase<IRemoteCommandManager>, IRemoteCommandManager
    {
        public RemoteCommandManager()
            : base( new InstanceContext(new RemoteCommandManagerCallback()),new ServiceEndpoint(ContractDescription.GetContract(typeof(IRemoteCommandManager)),
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/IRemoteCommandManager")))
        {
            
        }

        public void Register(string commandID, string text, string iconURL)
        {
            Channel.Register(commandID,text,iconURL);
        }

        public void UnRegister(string commandID)
        {
            throw new NotImplementedException();
        }

        public void CanExecuteChanged(string commandID)
        {
            throw new NotImplementedException();
        }
    }

    public class RemoteCommandManagerCallback : IRemoteCommandManagerCallback
    {
        public void Excute(string commandID)
        {
            throw new NotImplementedException();
        }

        public bool CanExcute(string commandID)
        {
            throw new NotImplementedException();
        }
    }
}
