using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel.Logging;
namespace Metaseed.MetaShell.Services
{
    public class LoggerMessage
    {
        public LoggerMessage(LogMessageEventArgs logMessage)
        {
            Time = logMessage.Time;
            Sender = logMessage.Log.TargetType.Name;
            Category = logMessage.LogEvent;
            Message = logMessage.Message + (logMessage.ExtraData ==null ? string.Empty : ";ExtraData:" + logMessage.ExtraData);
        }
        public LoggerMessage(string sender,LogEvent category,string message,object extraData)
        {
            Time = DateTime.Now;
            Sender = sender;
            Category = category;
            Message = message + (extraData == null ? string.Empty : ";ExtraData:" + extraData);
        }
        public LoggerMessage(string sender, LogEvent category, string message, object extraData,DateTime time)
        {
            Time = time;
            Sender = sender;
            Category = category;
            Message = message + (extraData == null ? string.Empty : ";ExtraData:" + extraData);
        }
        public DateTime Time { private set; get; }
        public String Sender { private set; get; }
        public LogEvent Category { private set; get; }
        public String Message {  set; get; }
        public override string ToString()
        {
          return   "DateTime:" + Time.ToShortTimeString() + "; Sender:"+Sender+"; Category: "+Category.ToString()+";"+ "Message:"+Message;
        }
    }
}
