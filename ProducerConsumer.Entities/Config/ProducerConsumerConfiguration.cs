using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Domain.Config
{
    public class ProducerConsumerConfiguration
    {
        public int ProducerCount { get; set; } = 1;
        public int ConsumerCount { get; set; } = 2;
        public int ProducerDelayMs { get; set; } = 500;
        public int ConsumerDelayMs { get; set; } = 800;
        public int MinValue { get; set; } = 1;
        public int MaxValue { get; set; } = 1000;
        public int MaxQueueSize { get; set; } = 100;
    }
}
