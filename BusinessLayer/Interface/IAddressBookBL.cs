using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using System.Collections.Generic;

namespace BusinessLayer.Interface
{
    /// <summary>
    /// Interface for Address Book Business Layer.
    /// Defines CRUD operations for managing address book contacts and authhentication
    /// </summary>
    public interface IAddressBookBL
    {
        IEnumerable<AddressBookModel> GetAllContacts();
        AddressBookModel GetContactById(int id);
        AddressBookModel AddContact(AddressBookModel contact);
        bool UpdateContact(int id, AddressBookModel contact);
        bool DeleteContact(int id);
        string Register(UserDTO userDto);
        string Login(string email, string password);
    }
}