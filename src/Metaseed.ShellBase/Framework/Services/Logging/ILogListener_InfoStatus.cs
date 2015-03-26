using System.Collections.ObjectModel;

namespace Metaseed.MetaShell.Services
{
    public interface ILoggerListener_Info 
    {
        ObservableCollection<LoggerMessage> Messages { get; }
    }
}
