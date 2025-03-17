using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Service;
using ModelLayer.DTO;
using RepositoryLayer.Service;
using RepositoryLayer;
using Microsoft.Extensions.Configuration;

namespace AddressBookTests.Service
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AddressBookBL _authService;
        private AddressBookDbContext _dbContext;
        private IConfiguration _configuration; // ✅ Ensure config for JWT
        private JWTService _jwtService;  // ✅ JWT dependency added

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AddressBookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ✅ Unique DB for each test
                .Options;

            _dbContext = new AddressBookDbContext(options);

            // ✅ Fake JWT Configuration
            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:Key", "ThisIsASecretKeyForTestingPurposes"},
                {"JWT:Issuer", "TestIssuer"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _jwtService = new JWTService((Microsoft.Extensions.Options.IOptions<JWTOptions>)_configuration); // ✅ Fix: Inject JWT Service

            var addressBookRepository = new AddressBookRL(_dbContext);

            var emailService = new EmailService(); // Ensure EmailService has a parameterless constructor
            _authService = new AddressBookBL(addressBookRepository, _jwtService, emailService);

        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void RegisterUser_Should_Return_Success_Message()
        {
            // Arrange
            var user = new UserDTO
            {
                Name = "Vanshita",
                Email = "khantalvanshita@gmail.com",
                Password = "Vanshita123"
            };

            // Act
            var result = _authService.Register(user);

            // Assert
            Assert.IsNotNull(result, "Registration failed, returned null.");
            Assert.IsInstanceOf<string>(result, "Return type should be a string.");
            Assert.IsTrue(result.Contains("success", StringComparison.OrdinalIgnoreCase), "Success message not found.");
        }

        [Test]
        public void LoginUser_Should_Return_JWT_Token()
        {
            // Arrange: Register user first
            var user = new UserDTO
            {
                Name = "Vanshita",
                Email = "khantalvanshita@gmail.com",
                Password = "Vanshita123"
            };
            _authService.Register(user); // ✅ Ensure user exists before login

            // Act
            var result = _authService.Login(user.Email, user.Password);

            // Assert
            Assert.IsNotNull(result, "Login failed, returned null.");
            Assert.IsInstanceOf<string>(result, "Return type should be a JWT token string.");
            Assert.IsTrue(result.Length > 10, "JWT token is too short.");
        }

        [Test]
        public void LoginUser_Should_Return_Null_If_Credentials_Are_Incorrect()
        {
            // Act
            var result = _authService.Login("invalid@example.com", "WrongPass");

            // Assert
            Assert.IsNull(result, "Login should return null for invalid credentials.");
        }
    }
}