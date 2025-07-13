using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Events
{
    public class ItemDequeuedEventArgs<T> : EventArgs
    {
        public T Item { get; }
        public int QueueSize { get; }
        public DateTime Timestamp { get; }

        public ItemDequeuedEventArgs(T item, int queueSize)
        {
            Item = item;
            QueueSize = queueSize;
            Timestamp = DateTime.Now;
        }
    }
}
