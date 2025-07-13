using ProducerConsumer.Domain.Config;
using ProducerConsumer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Interfaces
{
    public interface IProducerConsumerManager
    {
        Task StartAsync(ProducerConsumerConfiguration config);
        Task StopAsync();
        bool IsRunning { get; }

        IEnumerable<ProducerStatistics> GetProducerStatistics();
        IEnumerable<ConsumerStatistics> GetConsumerStatistics();
        QueueStatistics GetQueueStatistics();

        event EventHandler<ProducerStatistics> ProducerStatisticsUpdated;
        event EventHandler<ConsumerStatistics> ConsumerStatisticsUpdated;
        event EventHandler<QueueStatistics> QueueStatisticsUpdated;
    }
}
