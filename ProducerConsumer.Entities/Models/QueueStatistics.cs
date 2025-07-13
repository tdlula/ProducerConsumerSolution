namespace ProducerConsumer.Domain.Models
{
    public class QueueStatistics
    {
        public int CurrentSize { get; set; }
        public int MaxSize { get; set; }
        public int TotalEnqueued { get; set; }
        public int TotalDequeued { get; set; }
        public TimeSpan AverageWaitTime { get; set; }
    }
}
