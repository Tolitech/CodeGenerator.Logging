# Logging
Logging library used in projects created by the Code Generator tool.

This project contains abstract classes for implementing and customizing logging providers. 

Tolitech Code Generator Tool: [http://www.tolitech.com.br](https://www.tolitech.com.br/)

Examples:

```
public class LogProvider : LoggerProvider
{
    public override bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public override void WriteLog(LogEntry Info)
    {
        Console.WriteLine($"ActionName = {Info.ActionName}");
        Console.WriteLine($"Category = {Info.Category}");
        Console.WriteLine($"FilePath = {Info.FilePath}");
        Console.WriteLine($"LineNumber = {Info.LineNumber}");
        Console.WriteLine($"UserName = {Info.UserName}");
        Console.WriteLine($"HostName = {LogEntry.HostName}");
    }
}
```