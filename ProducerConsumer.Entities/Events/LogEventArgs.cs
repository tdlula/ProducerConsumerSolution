using ProducerConsumer.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Events
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; }
        public LogLevel Level { get; }
        public DateTime Timestamp { get; }

        public LogEventArgs(string message, LogLevel level)
        {
            Message = message;
            Level = level;
            Timestamp = DateTime.Now;
        }
    }
}
