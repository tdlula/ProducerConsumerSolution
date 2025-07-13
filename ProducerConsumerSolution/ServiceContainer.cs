using ProducerConsumer.Domain.Models;
using ProducerConsumer.Infrastructure;
using ProducerConsumer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumerSolution.UI
{
    public class ServiceContainer
    {
        public static MainForm CreateMainForm()
        {
            // Create infrastructure services
            var queue = new ThreadSafeQueue<QueueItem>();
            var logger = new LoggerService();
            var queueService = new QueueService(queue);

            // Create application services
            var manager = new ProducerConsumerManager(queue, logger, queueService);

            // Create presentation layer
            return new MainForm(manager, logger, queueService);
        }
    }
}
