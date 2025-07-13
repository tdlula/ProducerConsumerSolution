using ProducerConsumer.Domain.Interfaces;
using ProducerConsumer.Domain.Models;

namespace ProducerConsumer.Services
{
    public class QueueService : IQueueService
    {
        private readonly IThreadSafeQueue<QueueItem> _queue;
        private int _totalEnqueued;
        private int _totalDequeued;

        public event EventHandler<QueueStatistics> StatisticsUpdated;

        public QueueService(IThreadSafeQueue<QueueItem> queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));

            _queue.ItemEnqueued += (s, e) => {
                _totalEnqueued++;
                StatisticsUpdated?.Invoke(this, GetStatistics());
            };

            _queue.ItemDequeued += (s, e) => {
                _totalDequeued++;
                StatisticsUpdated?.Invoke(this, GetStatistics());
            };
        }

        public QueueStatistics GetStatistics()
        {
            return new QueueStatistics
            {
                CurrentSize = _queue.Count,
                TotalEnqueued = _totalEnqueued,
                TotalDequeued = _totalDequeued
            };
        }

        public QueueItem[] GetCurrentItems(int maxItems = 20)
        {
            return _queue.ToArray().Take(maxItems).ToArray();
        }
    }
}
