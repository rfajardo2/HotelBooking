using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class EmailService
    {
        private readonly string _apiKey;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"] ?? "";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("API Key de SendGrid no configurada.");
            }

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("rfajardomerco@gmail.com", "Hotel Booking");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error enviando email: {response.StatusCode}");
            }
        }
    }
}
