using System.ComponentModel.DataAnnotations;

namespace MyEmailWebApi.DTO
{
    public class SendEmailRequest
    {
        public string? Body { get; set; }

        [Required]
        public string? Recipient { get; set; }

        //public DateTime SentAt { get; set; }

        public string? Subject { get; set; }
    }
}
