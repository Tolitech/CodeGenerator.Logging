using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Tolitech.CodeGenerator.Logging
{
    public class LogEntry
    {
        public LogEntry()
        {
            TimeStampUtc = DateTime.UtcNow;
            UserName = Environment.UserName;
        }

        static public readonly string StaticHostName = System.Net.Dns.GetHostName();

        public string UserName { get; private set; }
        
        public string HostName { get { return StaticHostName; } }
        
        public DateTime TimeStampUtc { get; private set; }
        
        public string Category { get; set; }
        
        public LogLevel Level { get; set; }
        
        public string Text { get; set; }
        
        public Exception Exception { get; set; }
        
        public EventId EventId { get; set; }
        
        public object State { get; set; }
        
        public string StateText { get; set; }

        public string ActivityId { get; set; }

        public string UserId { get; set; }

        public string LoginName { get; set; }

        public string ActionId { get; set; }

        public string ActionName { get; set; }

        public string RequestId { get; set; }

        public string RequestPath { get; set; }

        public string Sql { get; set; }

        public string Parameters { get; set; }

        public string OriginalProperties { get; set; }

        public Dictionary<string, object> StateProperties { get; set; }
        
        public List<LogScopeInfo> Scopes { get; set; }

        public List<string> FilePath { get; set; }

        public List<string> LineNumber { get; set; }
    }
}
