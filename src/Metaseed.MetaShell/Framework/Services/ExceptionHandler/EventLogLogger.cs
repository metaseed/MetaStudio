using System.Diagnostics;
using System.Reflection;

namespace Metaseed.MetaShell.Services
{
  /// <summary>Logs errors to the application event log</summary>
  public class EventLogLogger : LoggerImplementation
  {
    /// <summary>Logs the specified error.</summary>
    /// <param name="error">The error to log.</param>
    public override void LogError(string error)
    {
      EventLog log = new EventLog("Application");
      log.Source = Assembly.GetExecutingAssembly().ToString();
      log.WriteEntry(error, EventLogEntryType.Error);
    }
  }
}
