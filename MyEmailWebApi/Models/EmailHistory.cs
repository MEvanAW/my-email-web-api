namespace MyEmailWebApi.Models
{
    public class EmailHistory
    {
        public Guid ID { get; set; }

        public string? Body { get; set; }

        public required string Recipient { get; set; }

        public DateTime SentAt { get; set; }

        public string? Subject { get; set; }
    }
}
