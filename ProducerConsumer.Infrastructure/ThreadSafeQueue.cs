using ProducerConsumer.Domain.Events;
using ProducerConsumer.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Infrastructure
{
    public class ThreadSafeQueue<T> : IThreadSafeQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly object _lockObject = new object();

        public event EventHandler<ItemEnqueuedEventArgs<T>> ItemEnqueued;
        public event EventHandler<ItemDequeuedEventArgs<T>> ItemDequeued;

        public ThreadSafeQueue()
        {
            _queue = new ConcurrentQueue<T>();
            _semaphore = new SemaphoreSlim(0);
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            _semaphore.Release();
            ItemEnqueued?.Invoke(this, new ItemEnqueuedEventArgs<T>(item, Count));
        }

        public bool TryDequeue(out T item)
        {
            bool result = _queue.TryDequeue(out item);
            if (result)
            {
                ItemDequeued?.Invoke(this, new ItemDequeuedEventArgs<T>(item, Count));
            }
            return result;
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            if (_queue.TryDequeue(out T item))
            {
                ItemDequeued?.Invoke(this, new ItemDequeuedEventArgs<T>(item, Count));
                return item;
            }
            throw new InvalidOperationException("Queue is empty after semaphore wait");
        }

        public int Count => _queue.Count;
        public bool IsEmpty => _queue.IsEmpty;
        public T[] ToArray() => _queue.ToArray();
    }
}
