using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using SlickdealsNotifier.Models;
using System;
using System.Threading.Tasks;

namespace SlickdealsNotifier.Notification
{
    public class SendGridEmailNotifier : IDealNotifier
    {
        private readonly ILogger<SendGridEmailNotifier> _logger;

        public SendGridEmailNotifier(ILogger<SendGridEmailNotifier> logger)
        {
            _logger = logger;
        }


        public async Task<bool> Notify(Deal deal, ApplicationConfiguration applicationConfiguration)
        {
            var apiKey = Environment.GetEnvironmentVariable("SLICKDEALS_SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(applicationConfiguration.EmailFrom, "Slickdeals Notifier");
            var subject = $"New Slick Deal: {deal.Title}, price: {deal.Price}, at {deal.Store}";
            var to = new EmailAddress(applicationConfiguration.EmailTo, "Slickdeals Recipient");
            var plainTextContent = $"A new Slick Deal is Available: {Environment.NewLine}{deal}";
            var htmlContent = $"<a href=\"{ deal.Url}\">Link to SlickDeals</a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            try
            {
                _logger.LogDebug($"Sending deal email with content {msg.PlainTextContent}");

                var response = await client.SendEmailAsync(msg);

                _logger.LogDebug($"Email send attempted. Response Status Code: {response.StatusCode}");
                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email. Exception: {ex.Message}");
                return false;
            }
        }
    }
}
