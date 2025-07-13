using ProducerConsumer.Domain.Events;

namespace ProducerConsumer.Domain.Interfaces
{
    public interface IThreadSafeQueue<T>
    {
        void Enqueue(T item);
        Task<T> DequeueAsync(CancellationToken cancellationToken = default);
        bool TryDequeue(out T item);
        int Count { get; }
        bool IsEmpty { get; }
        T[] ToArray();

        event EventHandler<ItemEnqueuedEventArgs<T>> ItemEnqueued;
        event EventHandler<ItemDequeuedEventArgs<T>> ItemDequeued;
    }
}
