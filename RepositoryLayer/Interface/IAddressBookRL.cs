using ModelLayer.Model;
using RepositoryLayer.Entity;
using System.Collections.Generic;

namespace RepositoryLayer.Interface
{
    /// <summary>
    /// Interface for Address Book Repository Layer.
    /// Defines CRUD operations for managing address book contacts and authentication
    /// </summary>
    public interface IAddressBookRL
    {
        IEnumerable<AddressBookModel> GetAll();
        AddressBookModel GetById(int id);
        AddressBookModel AddContact(AddressBookModel contact);
        bool UpdateContact(int id, AddressBookModel contact);
        bool DeleteContact(int id);
        AddressBookEntity RegisterUser(AddressBookEntity user);
        AddressBookEntity GetUserByEmail(string email);
    }
}