using System.Windows.Input;

namespace Metaseed.MVVM.Commands
{
    public interface IRemoteCommand : ICommand
    {
        string ID { get; set; }
        string UIType { get; set; }
        string UIData { get; set; }
    }
}
