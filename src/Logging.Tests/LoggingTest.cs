using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;
using Tolitech.CodeGenerator.Logging.Tests.Providers;

namespace Tolitech.CodeGenerator.Logging.Tests
{
    public class LoggingTest
    {
        private readonly ILogger<LoggingTest> _logger;

        public LoggingTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var logLevel = (LogLevel)config.GetSection("Logging:File:LogLevel").GetValue(typeof(LogLevel), "Default");

            var loggerFactory = LoggerFactory.Create(logger =>
            {
                logger
                    .AddConfiguration(config.GetSection("Logging"));

                var provider = new LogProvider();
                logger.AddProvider(provider);
            });

            _logger = loggerFactory.CreateLogger<LoggingTest>();
        }

        [Fact(DisplayName = "Logging - IsEnabled - Valid")]
        public void Logging_IsEnabled_Valid()
        {
            var provider = new LogProvider();
            bool b = provider.IsEnabled(LogLevel.Trace);
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

        [Fact(DisplayName = "Logging - Log - Valid")]
        public void Logging_Log_Valid()
        {
            _logger.Log(LogLevel.Trace, "{test1} {test2}", "param1", "param2");
            _logger.LogInformation("test");
            
            var disposable = _logger.BeginScope("test {test1} {test2}", "param1", "param2");
            disposable.Dispose();

            bool b = _logger.IsEnabled(LogLevel.Trace);
            Assert.True(b);
        }
    }
}
