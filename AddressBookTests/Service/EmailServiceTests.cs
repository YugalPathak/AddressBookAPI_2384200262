using NUnit.Framework;
using BusinessLayer.Service;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using BusinessLayer.Interface;

namespace AddressBookTests.Service
{
    [TestFixture]
    public class EmailServiceTests
    {
        private IEmailService emailService;

        [SetUp]
        public void Setup()
        {
            // ✅ Create a fake configuration for EmailSettings
            var inMemorySettings = new Dictionary<string, string>
            {
                {"EmailSettings:SmtpServer", "smtp.testserver.com"},
                {"EmailSettings:SmtpPort", "587"},
                {"EmailSettings:SenderEmail", "testsender@example.com"},
                {"EmailSettings:SenderPassword", "testpassword"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            emailService = new EmailService(configuration); // ✅ Pass fake config
        }

        [Test]
        public void SendEmailShouldNotThrowException()
        {
            // Arrange
            string email = "khantalvanshita@gmail.com";
            string resetToken = "dummy-reset-token-123";

            // Act & Assert ✅ Test should pass if no exception is thrown
            Assert.DoesNotThrow(() => emailService.SendPasswordResetEmail(email, resetToken),
                "Email sending method threw an exception.");
        }
    }
}