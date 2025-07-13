using ProducerConsumer.Domain.Enum;
using ProducerConsumer.Domain.Events;
using ProducerConsumer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Infrastructure
{
    public class LoggerService : ILoggerService
    {
        public event EventHandler<LogEventArgs> LogEntryAdded;

        public void LogInfo(string message) => AddLogEntry(message, LogLevel.Info);
        public void LogWarning(string message) => AddLogEntry(message, LogLevel.Warning);
        public void LogError(string message) => AddLogEntry(message, LogLevel.Error);
        public void LogDebug(string message) => AddLogEntry(message, LogLevel.Debug);

        private void AddLogEntry(string message, LogLevel level)
        {
            LogEntryAdded?.Invoke(this, new LogEventArgs(message, level));
        }
    }
}
