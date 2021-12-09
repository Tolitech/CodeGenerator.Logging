using System;
using Microsoft.Extensions.Logging;

namespace Tolitech.CodeGenerator.Logging.Tests.Providers
{
    public class LogProvider : LoggerProvider
    {
        public LogProvider()
        {
            
        }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public override void WriteLog(LogEntry Info)
        {
            Console.WriteLine($"ActionId = {Info.ActionId}");
            Console.WriteLine($"ActionName = {Info.ActionName}");
            Console.WriteLine($"ActivityId = {Info.ActivityId}");
            Console.WriteLine($"Category = {Info.Category}");
            Console.WriteLine($"EventId = {Info.EventId}");
            Console.WriteLine($"Exception = {Info.Exception}");
            Console.WriteLine($"FilePath = {Info.FilePath}");
            Console.WriteLine($"Level = {Info.Level}");
            Console.WriteLine($"LineNumber = {Info.LineNumber}");
            Console.WriteLine($"LoginName = {Info.LoginName}");
            Console.WriteLine($"OriginalProperties = {Info.OriginalProperties}");
            Console.WriteLine($"Parameters = {Info.Parameters}");
            Console.WriteLine($"RequestId = {Info.RequestId}");
            Console.WriteLine($"RequestPath = {Info.RequestPath}");
            Console.WriteLine($"Scopes = {Info.Scopes}");
            Console.WriteLine($"Sql = {Info.Sql}");
            Console.WriteLine($"State = {Info.State}");
            Console.WriteLine($"StateProperties = {Info.StateProperties}");
            Console.WriteLine($"StateText = {Info.StateText}");
            Console.WriteLine($"Text = {Info.Text}");
            Console.WriteLine($"TimeStampUtc = {Info.TimeStampUtc}");
            Console.WriteLine($"UserId = {Info.UserId}");
            Console.WriteLine($"UserName = {Info.UserName}");
            Console.WriteLine($"HostName = {LogEntry.HostName}");
        }
    }
}
