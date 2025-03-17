using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Net;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Service responsible for sending emails, specifically for password reset functionality.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="configuration">Configuration settings to access SMTP details.</param>
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Sends a password reset email to the user.
        /// </summary>
        /// <param name="email">Recipient's email address.</param>
        /// <param name="resetToken">Reset token to be included in the email body.</param>
        public void SendPasswordResetEmail(string email, string resetToken)
        {
            try
            {
                // Retrieve SMTP configuration from appsettings.json
                string smtpServer = _configuration["EmailSettings:SmtpServer"] ?? throw new Exception("SMTP Server is missing in configuration.");
                string smtpPortStr = _configuration["EmailSettings:SmtpPort"] ?? throw new Exception("SMTP Port is missing in configuration.");
                string senderEmail = _configuration["EmailSettings:SenderEmail"] ?? throw new Exception("Sender Email is missing in configuration.");
                string senderPassword = _configuration["EmailSettings:SenderPassword"] ?? throw new Exception("Sender Password is missing in configuration.");

                if (!int.TryParse(smtpPortStr, out int smtpPort))
                {
                    throw new Exception("Invalid SMTP Port. It should be a valid integer.");
                }

                // Create the email message
                var mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Password Reset Request",
                    Body = $"Use this token to reset your password: {resetToken}",
                    IsBodyHtml = true
                };

                mail.To.Add(email);

                // Configure and send the email
                using (var smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send password reset email: {ex.Message}", ex);
            }
        }
    }
}