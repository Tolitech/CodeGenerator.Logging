using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Tolitech.CodeGenerator.Logging
{
    internal class Logger : ILogger
    {
        public Logger(LoggerProvider Provider, string Category)
        {
            this.Provider = Provider;
            this.Category = Category;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return Provider.ScopeProvider.Push(state);
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return Provider.IsEnabled(logLevel);
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if ((this as ILogger).IsEnabled(logLevel))
            {
                LogEntry info = new LogEntry();
                info.Category = this.Category;
                info.Level = logLevel;
                info.Text = exception?.Message ?? state.ToString();
                info.Exception = exception;
                info.EventId = eventId;
                info.State = state;

                if (state is string)
                {
                    info.StateText = state.ToString();
                }
                else if (state is IEnumerable<KeyValuePair<string, object>> Properties)
                {
                    info.StateProperties = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, object> item in Properties)
                    {
                        info.StateProperties[item.Key] = item.Value;
                    }
                }

                if (Provider.ScopeProvider != null)
                {
                    Provider.ScopeProvider.ForEachScope((value, loggingProps) =>
                    {
                        if (info.Scopes == null)
                            info.Scopes = new List<LogScopeInfo>();

                        LogScopeInfo Scope = new LogScopeInfo();
                        info.Scopes.Add(Scope);

                        if (value is string)
                        {
                            Scope.Text = value.ToString();
                        }
                        else if (value is IEnumerable<KeyValuePair<string, object>> props)
                        {
                            if (Scope.Properties == null)
                                Scope.Properties = new Dictionary<string, object>();

                            foreach (var pair in props)
                            {
                                Scope.Properties[pair.Key] = pair.Value;
                            }
                        }
                    },
                    state);
                }

                Provider.WriteLog(info);
            }
        }

        public LoggerProvider Provider { get; private set; }
        
        public string Category { get; private set; }
    }
}
