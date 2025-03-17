using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using BusinessLayer.Service;
using BusinessLayer.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using NUnit.Framework;
using RepositoryLayer.Entity;

namespace AddressBookTests.Service
{
    [TestFixture]
    public class JWTServiceTests
    {
        private IJWTService _jwtService;

        [SetUp]
        public void Setup()
        {
            // ✅ Fake JWT Configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:Key", "ThisIsASecretKeyForTestingPurposes"},
                {"JWT:Issuer", "TestIssuer"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _jwtService = new JWTService((Microsoft.Extensions.Options.IOptions<JWTOptions>)configuration); // ✅ Inject configuration
        }

        [Test]
        public void GenerateToken_Should_Return_Valid_JWT()
        {
            // Arrange ✅ Create a test UserEntity object
            var user = new AddressBookEntity
            {
                Id = 1,
                Name = "Test",
                Email = "testuser@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@1234")
            };

            // Act
            var token = _jwtService.GenerateToken(user); // ✅ Pass the full UserEntity

            // Assert ✅ Ensure token is valid
            Assert.IsNotNull(token, "JWT Token generation failed.");
            Assert.IsInstanceOf<string>(token);
            Assert.IsTrue(token.Length > 10, "JWT Token is too short.");
        }


        [Test]
        public void Generated_Token_Should_Contain_Correct_Email()
        {
            // Arrange ✅ Create a test UserEntity object
            var user = new AddressBookEntity
            {
                Id = 1,
                Name = "User",
                Email = "testing@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@1234")
            };

            // Act ✅ Generate token using UserEntity
            var token = _jwtService.GenerateToken(user);

            // Decode JWT Token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert ✅ Check if email exists in token claims
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            Assert.IsNotNull(emailClaim, "Email claim is missing in JWT.");
            Assert.AreEqual(user.Email, emailClaim, "Token does not contain correct email.");
        }

    }
}
