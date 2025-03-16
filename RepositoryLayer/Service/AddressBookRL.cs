using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System.Collections.Generic;
using System.Linq;

namespace RepositoryLayer.Service
{
    /// <summary>
    /// Repository Layer for Address Book operations.
    /// Implements CRUD operations using an in-memory list.
    /// </summary>
    public class AddressBookRL : IAddressBookRL
    {
        /// <summary>
        /// Static list to store address book entries in memory.
        /// </summary>
        private static List<AddressBookModel> _addressBook = new List<AddressBookModel>();

        /// <summary>
        /// Static counter to generate unique IDs for contacts.
        /// </summary>
        private static int _idCounter = 1;

        private readonly AddressBookDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// </summary>
        public AddressBookRL(AddressBookDbContext context)
        {
            _context = context;
        }

        public void UpdateUser(AddressBookEntity user)
        {
            _context.AddressBookEntries.Update(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Registers a new user by adding them to the database.
        /// </summary>
        public AddressBookEntity RegisterUser(AddressBookEntity user)
        {
            _context.AddressBookEntries.Add(user);
            _context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Retrieves a user from the database using their email.
        /// </summary>

        public AddressBookEntity GetUserByEmail(string email)
        {
            return _context.AddressBookEntries.FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Saves the reset token and its expiry time for the user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="token">The generated reset token.</param>
        public void SaveResetToken(int userId, string token)
        {
            var user = _context.AddressBookEntries.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.ResetToken = token ?? throw new ArgumentNullException(nameof(token)); // Ensure token is not null
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
                _context.SaveChanges();
            }
        }


        /// <summary>
        /// Retrieves the user ID associated with a valid reset token.
        /// </summary>
        /// <param name="token">The reset token.</param>
        /// <returns>The ID of the user if the token is valid; otherwise, returns null.</returns>
        public int GetUserIdByResetToken(string token)
        {
            var user = _context.AddressBookEntries.FirstOrDefault(u => u.ResetToken == token && u.ResetTokenExpiry > DateTime.UtcNow);
            return user.Id;
        }

        /// <summary>
        /// Updates the user's password and clears the reset token after successful reset.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="newPassword">The new hashed password to be set.</param>
        public void UpdatePassword(int userId, string newPassword)
        {
            var user = _context.AddressBookEntries.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.PasswordHash = newPassword; // Hashing should be done before calling this method
                user.ResetToken = null; // Clear reset token
                user.ResetTokenExpiry = null;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves all contacts from the address book.
        /// </summary>
        /// <returns>A list of all address book entries.</returns>
        public IEnumerable<AddressBookModel> GetAll()
        {
            return _addressBook.ToList();
        }

        /// <summary>
        /// Retrieves a contact by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <returns>The contact details if found; otherwise, null.</returns>
        public AddressBookModel GetById(int id)
        {
            return _addressBook.SingleOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Adds a new contact to the address book.
        /// </summary>
        /// <param name="contact">The contact details to add.</param>
        /// <returns>The newly added contact with a unique ID.</returns>
        public AddressBookModel AddContact(AddressBookModel contact)
        {
            contact.Id = _idCounter++;
            _addressBook.Add(contact);
            return contact;
        }

        /// <summary>
        /// Updates an existing contact in the address book.
        /// </summary>
        /// <param name="id">The ID of the contact to update.</param>
        /// <param name="contact">The updated contact details.</param>
        /// <returns>True if the update is successful; otherwise, false.</returns>
        public bool UpdateContact(int id, AddressBookModel contact)
        {
            var existingContact = _addressBook.FirstOrDefault(c => c.Id == id);
            if (existingContact == null)
                return false;

            existingContact.FirstName = contact.FirstName;
            existingContact.LastName = contact.LastName;
            existingContact.Email = contact.Email;
            existingContact.Phone = contact.Phone;
            existingContact.Address = contact.Address;

            return true;
        }

        /// <summary>
        /// Deletes a contact from the address book.
        /// </summary>
        /// <param name="id">The ID of the contact to delete.</param>
        /// <returns>True if the deletion is successful; otherwise, false.</returns>
        public bool DeleteContact(int id)
        {
            var contact = _addressBook.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return false;

            _addressBook.Remove(contact);
            return true;
        }

        private class RedisCacheService
        {
        }
    }
}