using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace JPFData.Models.Email
{
    public class SendGridEmailService
    {
        private readonly SendGridClient _client;
        private readonly string _apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");


        public SendGridEmailService()
        {
            _client = new SendGridClient(_apiKey);
        }

        public async Task Send(EmailContract contract)
        {
            var emailMessage = new SendGridMessage();
            emailMessage.From = new EmailAddress(contract.FromEmailAddress, contract.Alias);
            emailMessage.Subject = contract.Subject;
            emailMessage.AddTo(new EmailAddress(contract.ToEmailAddress));

            if (!string.IsNullOrWhiteSpace(contract.HtmlContent))
                emailMessage.HtmlContent = contract.HtmlContent;
            if (!string.IsNullOrWhiteSpace(contract.PlainTextContent))
                emailMessage.PlainTextContent = contract.PlainTextContent;
            if (!string.IsNullOrWhiteSpace(contract.BccEmailAddress))
                emailMessage.AddBcc(new EmailAddress(contract.BccEmailAddress));
            if (!string.IsNullOrWhiteSpace(contract.CcEmailAddress))
                emailMessage.AddBcc(new EmailAddress(contract.CcEmailAddress));


              var response = await _client.SendEmailAsync(emailMessage);
        }
    }
}