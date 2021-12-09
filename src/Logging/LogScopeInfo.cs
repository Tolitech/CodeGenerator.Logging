using System;

namespace Tolitech.CodeGenerator.Logging
{
    public class LogScopeInfo
    {
        public string? Text { get; set; }

        public Dictionary<string, object>? Properties { get; set; }
    }
}
