using ModelLayer.Model;
using System.Collections.Generic;

namespace BusinessLayer.Interface
{
    /// <summary>
    /// Interface for Address Book Business Layer.
    /// Defines CRUD operations for managing address book contacts.
    /// </summary>
    public interface IAddressBookBL
    {
        IEnumerable<AddressBookModel> GetAllContacts();
        AddressBookModel GetContactById(int id);
        AddressBookModel AddContact(AddressBookModel contact);
        bool UpdateContact(int id, AddressBookModel contact);
        bool DeleteContact(int id);
    }
}