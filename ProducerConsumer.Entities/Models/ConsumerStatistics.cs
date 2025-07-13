namespace ProducerConsumer.Domain.Models
{
    public class ConsumerStatistics
    {
        public string Id { get; set; }
        public int TotalConsumed { get; set; }
        public int ConsumptionRate { get; set; } // items per second
        public DateTime LastConsumed { get; set; }
        public bool IsActive { get; set; }
    }
}
