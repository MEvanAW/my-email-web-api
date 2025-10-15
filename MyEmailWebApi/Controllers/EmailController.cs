using Microsoft.AspNetCore.Mvc;
using MyEmailWebApi.DTO;

namespace MyEmailWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private const string BAD_BODY = "Missing attribute or body format is not recognized.";
        private readonly ILogger<EmailController> _logger;

        public EmailController(ILogger<EmailController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "SendEmail")]
        public IActionResult SendEmail([FromBody] SendEmailRequest request)
        {
            return Ok(request);
        }
    }
}
