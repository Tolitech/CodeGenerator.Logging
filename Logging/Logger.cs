using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

                if (Activity.Current != null)
                {
                    var userIdTag = Activity.Current.Tags.FirstOrDefault(x => x.Key == "UserId");
                    var usernameTag = Activity.Current.Tags.FirstOrDefault(x => x.Key == "Username");

                    info.ActivityId = Activity.Current.RootId;
                    info.UserId = userIdTag.Value;
                    info.LoginName = usernameTag.Value;
                }

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

                        if (item.Key == "filePath")
                            info.FilePath = item.Value.ToString();
                        else if (item.Key == "memberName")
                            info.MemberName = item.Value.ToString();
                        else if (item.Key == "lineNumber")
                            info.LineNumber = item.Value.ToString();
                        else if (item.Key == "sql")
                            info.Sql = item.Value.ToString();
                        else if (item.Key == "parameters")
                            info.Parameters = item.Value.ToString();
                    }

                    info.OriginalProperties = Properties.ToString();
                }

                if (Provider.ScopeProvider != null)
                {
                    Provider.ScopeProvider.ForEachScope((value, loggingProps) =>
                    {
                        if (info.Scopes == null)
                            info.Scopes = new List<LogScopeInfo>();

                        LogScopeInfo scope = new LogScopeInfo();
                        info.Scopes.Add(scope);

                        if (value is string)
                        {
                            scope.Text = value.ToString();
                        }
                        else if (value is IEnumerable<KeyValuePair<string, object>> props)
                        {
                            if (scope.Properties == null)
                                scope.Properties = new Dictionary<string, object>();

                            foreach (var pair in props)
                            {
                                scope.Properties[pair.Key] = pair.Value;

                                if (pair.Key == "ActionId")
                                    info.ActionId = pair.Value.ToString();
                                else if (pair.Key == "ActionName")
                                    info.ActionName = pair.Value.ToString();
                                else if (pair.Key == "RequestId")
                                    info.RequestId = pair.Value.ToString();
                                else if (pair.Key == "RequestPath")
                                    info.RequestPath = pair.Value.ToString();
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
