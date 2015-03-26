
using System;
namespace Metaseed.MetaShell.Services
{
    public interface IExceptionHandler
    {
        event EventHandler SaveOnException;
        void AddLogger(LoggerImplementation logger);
    }
}
