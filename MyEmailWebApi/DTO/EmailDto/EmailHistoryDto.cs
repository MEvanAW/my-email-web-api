namespace MyEmailWebApi.DTO.EmailDto
{
    public class EmailHistoryDto : SendEmailRequest
    {
        public DateTime SentAt { get; set; }
    }
}
