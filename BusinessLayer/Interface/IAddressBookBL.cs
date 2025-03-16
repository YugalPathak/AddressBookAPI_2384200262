using ModelLayer.Model;
using RepositoryLayer.Entity;
using System.Collections.Generic;

namespace BusinessLayer.Interface
{
    /// <summary>
    /// Interface for Address Book Business Layer.
    /// Defines CRUD operations for managing address book contacts.
    /// </summary>
    public interface IAddressBookBL
    {
        IEnumerable<AddressBookEntry> GetAllContacts();
        AddressBookEntry GetContactById(int id);
        AddressBookEntry AddContact(AddressBookEntry contact);
        bool UpdateContact(int id, AddressBookEntry contact);
        bool DeleteContact(int id);
    }
}