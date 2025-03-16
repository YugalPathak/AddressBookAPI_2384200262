using ModelLayer.Model;
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
        private static List<AddressBookEntry> _addressBook = new List<AddressBookEntry>();

        /// <summary>
        /// Static counter to generate unique IDs for contacts.
        /// </summary>
        private static int _idCounter = 1;

        /// <summary>
        /// Retrieves all contacts from the address book.
        /// </summary>
        /// <returns>A list of all address book entries.</returns>
        public IEnumerable<AddressBookEntry> GetAll()
        {
            return _addressBook.ToList();
        }

        /// <summary>
        /// Retrieves a contact by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <returns>The contact details if found; otherwise, null.</returns>
        public AddressBookEntry GetById(int id)
        {
            return _addressBook.SingleOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Adds a new contact to the address book.
        /// </summary>
        /// <param name="contact">The contact details to add.</param>
        /// <returns>The newly added contact with a unique ID.</returns>
        public AddressBookEntry AddContact(AddressBookEntry contact)
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
        public bool UpdateContact(int id, AddressBookEntry contact)
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
    }
}