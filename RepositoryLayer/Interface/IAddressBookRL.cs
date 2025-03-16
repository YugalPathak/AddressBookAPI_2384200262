using ModelLayer.Model;
using RepositoryLayer.Entity;
using System.Collections.Generic;

namespace RepositoryLayer.Interface
{
    /// <summary>
    /// Interface for Address Book Repository Layer.
    /// Defines CRUD operations for managing address book contacts.
    /// </summary>
    public interface IAddressBookRL
    {
        IEnumerable<AddressBookEntry> GetAll();
        AddressBookEntry GetById(int id);
        AddressBookEntry AddContact(AddressBookEntry contact);
        bool UpdateContact(int id, AddressBookEntry contact);
        bool DeleteContact(int id);
    }
}