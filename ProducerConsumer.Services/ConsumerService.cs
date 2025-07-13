using ProducerConsumer.Domain.Interfaces;
using ProducerConsumer.Domain.Models;

namespace ProducerConsumer.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly IThreadSafeQueue<QueueItem> _queue;
        private readonly ILoggerService _logger;
        private readonly string _consumerId;

        private int _delayMs;
        private int _totalConsumed;
        private DateTime _lastConsumed;
        private bool _isActive;

        public ConsumerService(IThreadSafeQueue<QueueItem> queue, ILoggerService logger, string consumerId)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _consumerId = consumerId;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isActive = true;
            _logger.LogInfo($"Consumer {_consumerId} started");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var item = await _queue.DequeueAsync(cancellationToken);

                    _totalConsumed++;
                    _lastConsumed = DateTime.Now;

                    _logger.LogDebug($"Consumer {_consumerId} consumed: {item.Value} (produced by {item.ProducerId})");

                    // Simulate processing
                    await Task.Delay(_delayMs, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInfo($"Consumer {_consumerId} cancelled");
            }
            finally
            {
                _isActive = false;
                _logger.LogInfo($"Consumer {_consumerId} stopped");
            }
        }

        public void Stop()
        {
            // Stop is handled by cancellation token
        }

        public ConsumerStatistics GetStatistics()
        {
            return new ConsumerStatistics
            {
                Id = _consumerId,
                TotalConsumed = _totalConsumed,
                LastConsumed = _lastConsumed,
                IsActive = _isActive
            };
        }

        public void UpdateConfiguration(int delayMs)
        {
            _delayMs = delayMs;
        }
    }
}
