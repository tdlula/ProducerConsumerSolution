using ProducerConsumer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Interfaces
{
    public interface IConsumerService
    {
        Task StartAsync(CancellationToken cancellationToken);
        void Stop();
        ConsumerStatistics GetStatistics();
        void UpdateConfiguration(int delayMs);
    }
}
