using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Tolitech.CodeGenerator.Logging
{
    public abstract class LoggerProvider : IDisposable, ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, Logger> loggers = new();
        private IExternalScopeProvider? fScopeProvider;
        
        protected IDisposable? SettingsChangeToken;

        void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            fScopeProvider = scopeProvider;
        }

        ILogger ILoggerProvider.CreateLogger(string category)
        {
            return loggers.GetOrAdd(category, (category) => { return new Logger(this, category); });
        }

        void IDisposable.Dispose()
        {
            if (!this.IsDisposed)
            {
                try
                {
                    Dispose(true);
                }
                catch { }

                this.IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (SettingsChangeToken != null)
            {
                SettingsChangeToken.Dispose();
                SettingsChangeToken = null;
            }
        }

        ~LoggerProvider()
        {
            if (!this.IsDisposed)
            {
                Dispose(false);
            }
        }

        public abstract bool IsEnabled(LogLevel logLevel);

        public abstract void WriteLog(LogEntry Info);

        internal IExternalScopeProvider ScopeProvider
        {
            get
            {
                if (fScopeProvider == null)
                    fScopeProvider = new LoggerExternalScopeProvider();

                return fScopeProvider;
            }
        }

        public bool IsDisposed { get; protected set; }
    }
}