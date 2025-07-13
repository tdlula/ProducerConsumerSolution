namespace ProducerConsumer.Domain.Models
{
    public class QueueItem
    {
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProducerId { get; set; }
    }
}
