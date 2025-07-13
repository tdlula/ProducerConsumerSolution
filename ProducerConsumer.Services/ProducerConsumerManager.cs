using ProducerConsumer.Domain.Config;
using ProducerConsumer.Domain.Interfaces;
using ProducerConsumer.Domain.Models;


namespace ProducerConsumer.Services
{
    public class ProducerConsumerManager : IProducerConsumerManager
    {
        private readonly IThreadSafeQueue<QueueItem> _queue;
        private readonly ILoggerService _logger;
        private readonly IQueueService _queueService;

        private List<IProducerService> _producers;
        private List<IConsumerService> _consumers;
        private CancellationTokenSource _cancellationTokenSource;
        private Timer _statisticsTimer;

        public bool IsRunning { get; private set; }

        public event EventHandler<ProducerStatistics> ProducerStatisticsUpdated;
        public event EventHandler<ConsumerStatistics> ConsumerStatisticsUpdated;
        public event EventHandler<QueueStatistics> QueueStatisticsUpdated;

        public ProducerConsumerManager(IThreadSafeQueue<QueueItem> queue, ILoggerService logger, IQueueService queueService)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));

            _producers = new List<IProducerService>();
            _consumers = new List<IConsumerService>();
        }

        public async Task StartAsync(ProducerConsumerConfiguration config)
        {
            if (IsRunning) return;

            _cancellationTokenSource = new CancellationTokenSource();
            IsRunning = true;

            // Create producers
            _producers.Clear();
            for (int i = 0; i < config.ProducerCount; i++)
            {
                var producer = new ProducerService(_queue, _logger, $"Producer-{i + 1}");
                producer.UpdateConfiguration(config.ProducerDelayMs, config.MinValue, config.MaxValue);
                _producers.Add(producer);
            }

            // Create consumers
            _consumers.Clear();
            for (int i = 0; i < config.ConsumerCount; i++)
            {
                var consumer = new ConsumerService(_queue, _logger, $"Consumer-{i + 1}");
                consumer.UpdateConfiguration(config.ConsumerDelayMs);
                _consumers.Add(consumer);
            }

            // Start all producers and consumers
            var tasks = new List<Task>();
            tasks.AddRange(_producers.Select(p => p.StartAsync(_cancellationTokenSource.Token)));
            tasks.AddRange(_consumers.Select(c => c.StartAsync(_cancellationTokenSource.Token)));

            // Start statistics timer
            _statisticsTimer = new Timer(UpdateStatistics, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            _logger.LogInfo("Producer-Consumer system started");
        }

        public async Task StopAsync()
        {
            if (!IsRunning) return;

            _cancellationTokenSource?.Cancel();
            _statisticsTimer?.Dispose();

            IsRunning = false;
            _logger.LogInfo("Producer-Consumer system stopped");
        }

        private void UpdateStatistics(object state)
        {
            // Update producer statistics
            foreach (var producer in _producers)
            {
                ProducerStatisticsUpdated?.Invoke(this, producer.GetStatistics());
            }

            // Update consumer statistics
            foreach (var consumer in _consumers)
            {
                ConsumerStatisticsUpdated?.Invoke(this, consumer.GetStatistics());
            }

            // Update queue statistics
            QueueStatisticsUpdated?.Invoke(this, _queueService.GetStatistics());
        }

        public IEnumerable<ProducerStatistics> GetProducerStatistics()
        {
            return _producers.Select(p => p.GetStatistics());
        }

        public IEnumerable<ConsumerStatistics> GetConsumerStatistics()
        {
            return _consumers.Select(c => c.GetStatistics());
        }

        public QueueStatistics GetQueueStatistics()
        {
            return _queueService.GetStatistics();
        }
    }
}
