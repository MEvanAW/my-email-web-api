using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using MyEmailWebApi.DTO;

namespace MyEmailWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private const string BAD_BODY = "Missing attribute or body format is not recognized.";
        private const string CLASS = "class";
        private const string STYLE = "style";
        private readonly ILogger<EmailController> _logger;
        // docs: https://www.nuget.org/packages/HtmlSanitizer/#readme-body-tab
        private readonly HtmlSanitizer sanitizer;

        public EmailController(ILogger<EmailController> logger)
        {
            _logger = logger;
            sanitizer = new();
            sanitizer.AllowedTags.Add(STYLE);
            sanitizer.AllowedAttributes.Add(CLASS);
        }

        [HttpPost(Name = "SendEmail")]
        public IActionResult SendEmail([FromBody] SendEmailRequest request)
        {
            if (request.Body is not null)
            {
                // Sanitize the body to prevent XSS attack or similar. Will strip script tag and onload attribute
                request.Body = sanitizer.Sanitize(request.Body);
            }
            return Ok(request);
        }
    }
}
