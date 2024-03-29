﻿using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Tolitech.CodeGenerator.Logging
{
    internal class Logger : ILogger
    {
        public Logger(LoggerProvider provider, string category)
        {
            this.Provider = provider;
            this.Category = category;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return Provider.ScopeProvider.Push(state);
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return Provider.IsEnabled(logLevel);
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            if ((this as ILogger).IsEnabled(logLevel))
            {
                LogEntry info = new()
                {
                    Category = this.Category,
                    Level = logLevel,
                    Text = exception?.Message ?? state?.ToString(),
                    Exception = exception,
                    EventId = eventId,
                    State = state
                };

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
                    info.StateText = state?.ToString();
                }
                else if (state is IEnumerable<KeyValuePair<string, object>> Properties)
                {
                    info.StateProperties = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, object> item in Properties)
                    {
                        if (item.Key == "sql")
                            info.Sql = item.Value.ToString();
                        else if (item.Key == "parameters")
                            info.Parameters = item.Value.ToString();
                        else
                            info.StateProperties[item.Key] = item.Value;
                    }

                    info.OriginalProperties = Properties.ToString();
                }

                if (Provider.ScopeProvider != null)
                {
                    Provider.ScopeProvider.ForEachScope((value, loggingProps) =>
                    {
                        if (info.Scopes == null)
                            info.Scopes = new List<LogScopeInfo>();

                        LogScopeInfo scope = new();
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
                                if (pair.Key == "ActionId")
                                    info.ActionId = pair.Value.ToString();
                                else if (pair.Key == "ActionName")
                                    info.ActionName = pair.Value.ToString();
                                else if (pair.Key == "RequestId")
                                    info.RequestId = pair.Value.ToString();
                                else if (pair.Key == "RequestPath")
                                    info.RequestPath = pair.Value.ToString();
                                else
                                    scope.Properties[pair.Key] = pair.Value;
                            }
                        }
                    },
                    state);
                }

                if (exception != null)
                {
                    info.FilePath = new List<string>();
                    info.LineNumber = new List<string>();

                    var frames = new StackTrace(exception, true)
                        .GetFrames()
                        .Where(x =>
                            !string.IsNullOrEmpty(x.GetFileName()))
                        .ToList();

                    foreach (var frame in frames)
                    {
                        string? fileName = frame.GetFileName();

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            info.FilePath.Add(fileName);
                            info.LineNumber.Add(frame.GetFileLineNumber().ToString());
                        }
                    }
                }

                Provider.WriteLog(info);
            }
        }

        public LoggerProvider Provider { get; private set; }

        public string Category { get; private set; }
    }
}