using ProducerConsumer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Interfaces
{
    public interface IProducerService
    {
        Task StartAsync(CancellationToken cancellationToken);
        void Stop();
        ProducerStatistics GetStatistics();
        void UpdateConfiguration(int delayMs, int minValue, int maxValue);
    }
}
