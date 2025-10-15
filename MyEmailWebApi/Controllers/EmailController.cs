using Ganss.Xss;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MyEmailWebApi.DTO;

namespace MyEmailWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        #region const
        private const string CLASS = "class";
        private const string FROM_ADDRESS = "FromAddress";
        private const string FROM_NAME = "FromName";
        private const string HOST = "Host";
        private const string PASSWORD = "Password";
        private const string PORT = "Port";
        private const string RECIPIENT = "Recipient";
        private const string SEND_EMAIL_RESPONSE_FORMAT = "Email has been sent to {0}.";
        private const string SMTP = "Smtp";
        private const string STYLE = "style";
        private const string USE_SSL = "UseSsl";
        private const string USERNAME = "Username";
        #endregion

        private readonly IConfigurationSection _smtpConfig;
        private readonly ILogger<EmailController> _logger;
        // docs: https://www.nuget.org/packages/HtmlSanitizer/#readme-body-tab
        private readonly HtmlSanitizer _sanitizer;

        public EmailController(ILogger<EmailController> logger, IConfiguration configRoot)
        {
            _smtpConfig = configRoot.GetSection(SMTP);
            _logger = logger;
            _sanitizer = new();
            _sanitizer.AllowedTags.Add(STYLE);
            _sanitizer.AllowedAttributes.Add(CLASS);
        }

        [HttpPost(Name = "SendEmail")]
        public IActionResult SendEmail([FromBody] SendEmailRequest request)
        {
            // Building message.
            var bodyBuilder = new BodyBuilder();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpConfig[FROM_NAME] ?? string.Empty, _smtpConfig[FROM_ADDRESS]));
            message.To.Add(new MailboxAddress(RECIPIENT, request.Recipient));
            if (request.Subject is not null)
            {
                bodyBuilder.TextBody = request.Subject;
                message.Subject = request.Subject;
            }
            if (request.Body is not null)
            {
                // Sanitize the body to prevent XSS attack or similar. Will strip script tag and onload attribute
                bodyBuilder.HtmlBody = _sanitizer.SanitizeDocument(request.Body);
            }
            message.Body = bodyBuilder.ToMessageBody();

            // Sending email.
            // docs: https://github.com/jstedfast/MailKit?tab=readme-ov-file#sending-messages
            using (var client = new SmtpClient())
            {
                client.Connect(_smtpConfig[HOST], _smtpConfig.GetValue<int>(PORT, 587), _smtpConfig.GetValue<bool>(USE_SSL, false));
                client.Authenticate(_smtpConfig[USERNAME], _smtpConfig[PASSWORD]);
                client.Send(message);
                client.Disconnect(true);
            }
            return Ok(string.Format(SEND_EMAIL_RESPONSE_FORMAT, request.Recipient));
        }
    }
}
