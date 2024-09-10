using Azure.Communication.Email;
using LawaCommunicationAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mail;

namespace LawaCommunicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunicationController : Controller
    {
        private readonly EmailService _emailService;

        public CommunicationController()
        {
            _emailService = new EmailService();
        }

        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            try
            {
               
                var result = await _emailService.SendEmailAsync(emailRequest.SenderEmail, emailRequest.CsvRecipientEmail, emailRequest.Subject, emailRequest.PlainTextBody,emailRequest.Attachments);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    // EmailRequest class to map the JSON body in the request
    public class EmailRequest
    {
        public string SenderEmail { get; set; }
        public string CsvRecipientEmail { get; set; }
        public string Subject { get; set; }
        public string PlainTextBody { get; set; }
        public IList<AttachmentDto> Attachments { get; set; }
    }

    public class AttachmentDto
    {
        public string FileName { get; set; }
        public string FileContentBase64 { get; set; }  // Base64 encoded file content
        public string MimeType { get; set; }           // MIME type (e.g., application/pdf, image/jpeg)
    }
}
