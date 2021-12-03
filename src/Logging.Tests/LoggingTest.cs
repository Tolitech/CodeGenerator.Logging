using System;
using Tolitech.CodeGenerator.Logging.Tests.Providers;
using Xunit;

namespace Tolitech.CodeGenerator.Logging.Tests
{
    public class LoggingTest
    {
        [Fact(DisplayName = "Logging - IsEnabled - Valid")]
        public void Logging_IsEnabled_Valid()
        {
            var provider = new LogProvider();
            bool b = provider.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace);
            Assert.True(b == true);
        }

        [Fact(DisplayName = "Logging - WriteLog - Valid")]
        public void Logging_WriteLog_Valid()
        {
            var provider = new LogProvider();
            var entry = new LogEntry();
            provider.WriteLog(entry);
            Assert.False(provider.IsDisposed);
        }
    }
}
