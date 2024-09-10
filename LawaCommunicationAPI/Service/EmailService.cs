
using Azure;
using Azure.Communication.Email;
using LawaCommunicationAPI.Controllers;
using static System.Net.WebRequestMethods;

namespace LawaCommunicationAPI.Service
{
    // https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/handle-email-events
    // https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email?pivots=programming-language-csharp&tabs=windows%2Cconnection-string%2Csend-email-and-get-status-async%2Csync-client
    public class EmailService
    {
        private readonly EmailClient _emailClient;

        public EmailService()
        {
            // Replace with your connection string
            // string connectionString = "endpoint=https://cs-fshp-dev-westus3-001.unitedstates.communication.azure.com/;accesskey=6uWfOLd8hREhbzq6xSd20fj5KviFHKxgEI5adZ3O62O3wLZHGoNzJQQJ99AHACULyCpYwemBAAAAAZCS5gYp";
            string connectionString = "endpoint=https://lawaemailsenderservice.unitedstates.communication.azure.com/;accesskey=9nS4d6ABeHYtXCqLb2MeNXyNHRqIN7RNkCIHDpQ6Qnlfl41Ma2LMJQQJ99AIACULyCpWYzN0AAAAAZCS4ljd";
            _emailClient = new EmailClient(connectionString);
        }

        // Method to send an email with attachments
        public async Task<EmailSendOperation> SendEmailAsync(
            string senderEmail,
            string csvRecipientEmail,
            string subject,
            string plainTextBody,
            IList<AttachmentDto> attachments)
        {
            var emailContent = new EmailContent(subject)
            {
                PlainText = plainTextBody
            };

            // Parse recipients
            List<EmailAddress> recipientEmails = csvRecipientEmail.Split(',')
                .Select(email => new EmailAddress(email.Trim())).ToList();
            var emailRecipients = new EmailRecipients(recipientEmails);

            // Create the email message
            var emailMessage = new EmailMessage(senderEmail, emailRecipients, emailContent);

            // Handle attachments if any are provided
            if (attachments != null && attachments.Any())
            {
                foreach (var attachmentDto in attachments)
                {
                    var attachment = new EmailAttachment(
                        attachmentDto.FileName,
                        attachmentDto.MimeType,
                        BinaryData.FromBytes(Convert.FromBase64String(attachmentDto.FileContentBase64)) // Convert byte[] to BinaryData
                    );
                    emailMessage.Attachments.Add(attachment);
                }
            }
            try
            {
                // Send the email
                var sendResult = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
                return sendResult;
            }
            catch (RequestFailedException ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }
    }
}
