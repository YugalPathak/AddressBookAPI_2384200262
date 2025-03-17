using BusinessLayer.Interface;
using BusinessLayer.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using BusinessLayer.Service;
using RepositoryLayer;
using ModelLayer.Model;

namespace AddressBookTests.Service
{
    [TestFixture]
    public class AddressBookBLTests
    {
        private AddressBookDbContext _dbContext;
        private IAddressBookBL _addressBookService;
        private JWTService jwtService;
        private EmailService emailService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AddressBookDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _dbContext = new AddressBookDbContext(options);

            var addressBookDbContextOptions = new DbContextOptionsBuilder<AddressBookDbContext>()
                .UseInMemoryDatabase(databaseName: "AddressBookDB")
                .Options;
            var addressBookDbContext = new AddressBookDbContext(addressBookDbContextOptions);

            IAddressBookRL addressBookRepo = new AddressBookRL(addressBookDbContext);
            _addressBookService = new AddressBookBL(addressBookRepo, jwtService, emailService);
        }


        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        // ✅ **Test 1: Add Contact**
        [Test]
        public void AddContact_Should_Add_New_Contact()
        {
            // Arrange
            var contact = new AddressBookModel
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Address = "New York"
            };

            // Act
            var result = _addressBookService.AddContact(contact);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual("Doe", result.LastName);
        }

        // ✅ **Test 2: Get Contact by ID**
        [Test]
        public void GetContactById_Should_Return_Correct_Contact()
        {
            // Arrange
            var contact = new AddressBookModel
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                Phone = "9876543210",
                Address = "California"
            };
            var addedContact = _addressBookService.AddContact(contact);

            // Act
            var result = _addressBookService.GetContactById(addedContact.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Alice", result.FirstName);
            Assert.AreEqual("Smith", result.LastName);
        }

        // ✅ **Test 3: Get All Contacts**
        [Test]
        public void GetAllContacts_Should_Return_All_Contacts()
        {
            // Arrange
            _addressBookService.AddContact(new AddressBookModel { FirstName = "A", LastName = "B", Email = "a@b.com" });
            _addressBookService.AddContact(new AddressBookModel { FirstName = "C", LastName = "D", Email = "c@d.com" });

            // Act
            var result = _addressBookService.GetAllContacts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void UpdateContact_Should_Modify_Existing_Contact()
        {
            // Arrange
            var contact = new AddressBookModel
            {
                FirstName = "Tom",
                LastName = "Hanks",
                Email = "tom.hanks@example.com",
                Phone = "1112223333",
                Address = "Jhansi"
            };
            var addedContact = _addressBookService.AddContact(contact);

            // Act
            addedContact.Phone = "9998887777";
            _addressBookService.UpdateContact(addedContact.Id, addedContact);

            // Fetch the updated contact again
            var updatedContact = _addressBookService.GetContactById(addedContact.Id);

            // Assert
            Assert.IsNotNull(updatedContact);
            Assert.AreEqual("9998887777", updatedContact.Phone);
        }


        // ✅ **Test 5: Delete Contact**
        [Test]
        public void DeleteContact_Should_Remove_Existing_Contact()
        {
            // Arrange
            var contact = new AddressBookModel
            {
                FirstName = "Chris",
                LastName = "Evans",
                Email = "chris.evans@example.com",
                Phone = "3334445555",
                Address = "Boston"
            };
            var addedContact = _addressBookService.AddContact(contact);

            // Act
            var result = _addressBookService.DeleteContact(addedContact.Id);
            var deletedContact = _addressBookService.GetContactById(addedContact.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(deletedContact);
        }
    }
}