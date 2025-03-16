using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using System.Collections.Generic;
using RepositoryLayer.Entity;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Business Logic Layer for Address Book operations.
    /// Acts as an intermediary between the Controller and Repository layers.
    /// </summary>
    public class AddressBookBL : IAddressBookBL
    {
        /// <summary>
        /// Repository layer dependency for accessing data operations.
        /// </summary>
        private readonly IAddressBookRL _addressBookRepository;

        /// <summary>
        /// Initializes a new instance of the AddressBookBL class.
        /// </summary>
        /// <param name="addressBookRepository">Repository layer dependency.</param>
        public AddressBookBL(IAddressBookRL addressBookRepository)
        {
            _addressBookRepository = addressBookRepository;
        }

        /// <summary>
        /// Retrieves all contacts from the address book.
        /// </summary>
        /// <returns>A list of all address book entries.</returns>
        public IEnumerable<AddressBookModel> GetAllContacts()
        {
            return _addressBookRepository.GetAll();
        }

        /// <summary>
        /// Retrieves a contact by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <returns>The contact details if found; otherwise, null.</returns>
        public AddressBookModel GetContactById(int id)
        {
            return _addressBookRepository.GetById(id);
        }

        /// <summary>
        /// Adds a new contact to the address book.
        /// </summary>
        /// <param name="contact">The contact details to add.</param>
        /// <returns>The newly added contact with a unique ID.</returns>
        public AddressBookModel AddContact(AddressBookModel contact)
        {
            return _addressBookRepository.AddContact(contact);
        }

        /// <summary>
        /// Updates an existing contact in the address book.
        /// </summary>
        /// <param name="id">The ID of the contact to update.</param>
        /// <param name="contact">The updated contact details.</param>
        /// <returns>True if the update is successful; otherwise, false.</returns>
        public bool UpdateContact(int id, AddressBookModel contact)
        {
            return _addressBookRepository.UpdateContact(id, contact);
        }

        /// <summary>
        /// Deletes a contact from the address book.
        /// </summary>
        /// <param name="id">The ID of the contact to delete.</param>
        /// <returns>True if the deletion is successful; otherwise, false.</returns>
        public bool DeleteContact(int id)
        {
            return _addressBookRepository.DeleteContact(id);
        }
    }
}