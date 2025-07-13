namespace ProducerConsumer.Domain.Models
{
    public class ProducerStatistics
    {
        public string Id { get; set; }
        public int TotalProduced { get; set; }
        public int ProductionRate { get; set; } // items per second
        public DateTime LastProduced { get; set; }
        public bool IsActive { get; set; }
    }
}
