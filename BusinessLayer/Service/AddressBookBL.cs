﻿using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using System.Collections.Generic;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;

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
        private readonly IJWTService _jwtService;

        /// <summary>
        /// Initializes a new instance of the AddressBookBL class.
        /// </summary>
        /// <param name="addressBookRepository">Repository layer dependency.</param>
        public AddressBookBL(IAddressBookRL addressBookRepository, IJWTService jwtService)
        {
            _addressBookRepository = addressBookRepository;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registers a new user, hashes their password, and stores it in the database.
        /// </summary>
        public string Register(UserDTO userDto)
        {
            var user = new AddressBookEntity
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            _addressBookRepository.RegisterUser(user);
            return "User registered successfully";
        }

        /// <summary>
        /// Registers a new user, hashes their password, and stores it in the database.
        /// </summary>
        public string Login(string email, string password)
        {
            var user = _addressBookRepository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return "Invalid credentials";

            return _jwtService.GenerateToken(user);
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