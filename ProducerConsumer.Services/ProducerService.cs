using ProducerConsumer.Domain.Models;
using ProducerConsumer.Domain.Interfaces;

namespace ProducerConsumer.Services
{
    public class ProducerService : IProducerService
    {
        private readonly IThreadSafeQueue<QueueItem> _queue;
        private readonly ILoggerService _logger;
        private readonly string _producerId;
        private readonly Random _random;

        private int _delayMs;
        private int _minValue;
        private int _maxValue;
        private int _totalProduced;
        private DateTime _lastProduced;
        private bool _isActive;
        private CancellationTokenSource _cancellationTokenSource;

        public ProducerService(IThreadSafeQueue<QueueItem> queue, ILoggerService logger, string producerId)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _producerId = producerId;
            _random = new Random();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isActive = true;
            _logger.LogInfo($"Producer {_producerId} started");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var item = new QueueItem
                    {
                        Value = _random.Next(_minValue, _maxValue),
                        CreatedAt = DateTime.Now,
                        ProducerId = _producerId
                    };

                    _queue.Enqueue(item);
                    _totalProduced++;
                    _lastProduced = DateTime.Now;

                    _logger.LogDebug($"Producer {_producerId} produced: {item.Value}");

                    await Task.Delay(_delayMs, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInfo($"Producer {_producerId} cancelled");
            }
            finally
            {
                _isActive = false;
                _logger.LogInfo($"Producer {_producerId} stopped");
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public ProducerStatistics GetStatistics()
        {
            return new ProducerStatistics
            {
                Id = _producerId,
                TotalProduced = _totalProduced,
                LastProduced = _lastProduced,
                IsActive = _isActive
            };
        }

        public void UpdateConfiguration(int delayMs, int minValue, int maxValue)
        {
            _delayMs = delayMs;
            _minValue = minValue;
            _maxValue = maxValue;
        }
    }
}
