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
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        void Register(string commandID, CommandUIData uiData );

        [OperationContract(IsOneWay = true)]
        void UnRegister(string commandID);

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
        [OperationContract]
        bool CanExcute(string commandID, object para);
    }

    [DataContract]
    public class ValidationFault
    {
        [DataMember]
        public bool Result { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
