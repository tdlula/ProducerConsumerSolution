using ProducerConsumer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Interfaces
{
    public interface IQueueService
    {
        QueueStatistics GetStatistics();
        QueueItem[] GetCurrentItems(int maxItems = 20);
        event EventHandler<QueueStatistics> StatisticsUpdated;
    }
}
