namespace MyEmailWebApi.DTO
{
    public class EmailHistoryDto : SendEmailRequest
    {
        public DateTime SentAt { get; set; }
    }
}
