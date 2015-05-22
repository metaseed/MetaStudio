using System.Runtime.Serialization;
using System.ServiceModel;

namespace Metaseed.MVVM.Commands
{
    /// <summary>
    /// Define a duplex service contract.
    /// A duplex contract consists of two interfaces.
    /// The primary interface is used to send messages from client to service.
    /// The callback interface is used to send messages from service back to client.
    /// </summary>
    [ServiceContract(Namespace = "http://Metaseed.MVVM.Commands", SessionMode = SessionMode.Required, CallbackContract = typeof(IRemoteCommandServiceCallback))]
    public interface IRemoteCommandService
    {
        /// <summary>
        /// FaultException<RemoteCommandFault> throw when could not register
        /// </summary>
        /// <param name="commandID"></param>
        /// <param name="uiData"></param>
        [OperationContract]
        [FaultContract(typeof(RemoteCommandFault))]
        void Register(string commandID,string uiType, string uiData );

        [OperationContract(IsOneWay = true)]
        void UnRegister(string commandID);

        /// <summary>
        /// trigger CanExecuteChanged event from client side
        /// </summary>
        /// <param name="commandID"></param>
        [OperationContract(IsOneWay = true)]
        void CanExecuteChanged(string commandID);
    }
    /// <summary>
    /// The callback interface is used to send messages from service back to client.
    /// </summary>
    public interface IRemoteCommandServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void Excute(string commandID, object para);
        /// <summary>
        /// query CanExcute status from serve side
        /// </summary>
        /// <param name="commandID"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        [OperationContract]
        bool CanExcute(string commandID, object para);
    }

    /// <summary>
    /// Data of remote comand API exception
    /// </summary>
    [DataContract]
    public class RemoteCommandFault
    {
        [DataMember]
        public bool Result { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
