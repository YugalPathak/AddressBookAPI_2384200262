using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using BusinessLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModelLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Service;
using AutoMapper;
using RepositoryLayer.Entity;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing address book contacts.
    /// Provides endpoints for CRUD operations.
    /// </summary>
    [ApiController]
    [Route("api/addressbook")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookService;
        private readonly RedisCacheService _redisCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookController"/> class.
        /// </summary>
        /// <param name="addressBookService">Business layer service for address book operations.</param>
        public AddressBookController(IAddressBookBL addressBookService, RedisCacheService redisCache)
        {
            _addressBookService = addressBookService;
            _redisCache = redisCache;
        }

        /// <summary>
        /// Registers a new user with the given details.
        /// </summary>
        /// <param name="userDto">The user data transfer object containing user details.</param>
        /// <returns>Returns a message indicating whether the registration was successful.</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDto)
        {
            var result = _addressBookService.Register(userDto);
            return Ok(new { message = result });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="userDto">UserDTO containing the email and password for authentication.</param>
        /// <returns>Returns a JWT token if authentication is successful, otherwise returns an unauthorized response.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDTO userDto)
        {
            var token = _addressBookService.Login(userDto.Email, userDto.Password);
            if (token == "Invalid credentials") return Unauthorized(new { message = token });
            return Ok(new { token });
        }

        /// <summary>
        /// Initiates the password reset process by generating a reset token
        /// and sending a password reset email to the user.
        /// </summary>
        /// <param name="request">Contains the user's email for password reset.</param>
        /// <returns>Returns an Ok response if the email is sent successfully.</returns>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            _addressBookService.ForgotPassword(request.Email);
            return Ok("Password reset email sent.");
        }

        /// <summary>
        /// Resets the user's password using the provided reset token and new password.
        /// </summary>
        /// <param name="request">Contains the reset token and new password.</param>
        /// <returns>Returns an Ok response if the password is reset successfully.</returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO request)
        {
            _addressBookService.ResetPassword(request.Token, request.NewPassword);
            return Ok("Password reset successfully.");
        }

        /// <summary>
        /// Gets protected data through token
        /// </summary>
        /// <returns>Return a message that it is a secure API</returns>
        [Authorize]
        [HttpGet("protected-data")]
        public IActionResult GetProtectedData()
        {
            return Ok(new { message = "This is a secure API!" });
        }

        /// <summary>
        /// Retrieves all contacts from the address book.
        /// </summary>
        /// <returns>A list of contacts.</returns>
        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            string cacheKey = "AddressBookContacts";

            // Check if data is in cache
            var cachedContacts = await _redisCache.GetCacheAsync<IEnumerable<AddressBookModel>>(cacheKey);
            if (cachedContacts != null)
            {
                return Ok(new { message = "Data from cache", contacts = cachedContacts });
            }

            // If not in cache, fetch from database
            var contacts = _addressBookService.GetAllContacts();

            // Store data in Redis cache
            await _redisCache.SetCacheAsync(cacheKey, contacts, TimeSpan.FromMinutes(10));

            return Ok(new { message = "Data from database", contacts });
        }

        /// <summary>
        /// Retrieves a contact by its ID.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <returns>The contact details if found; otherwise, NotFound.</returns>
        public async Task<IActionResult> GetContactById(int id)
        {
            string cacheKey = $"Contact_{id}";

            // Check if data is in cache
            var cachedContact = await _redisCache.GetCacheAsync<AddressBookModel>(cacheKey);
            if (cachedContact != null)
            {
                return Ok(new { message = "Data from cache", contact = cachedContact });
            }

            // If not in cache, fetch from database
            var contact = _addressBookService.GetContactById(id);
            if (contact == null)
            {
                return NotFound(new { message = "Contact not found" });
            }

            // Store data in Redis cache
            await _redisCache.SetCacheAsync(cacheKey, contact, TimeSpan.FromMinutes(10));

            return Ok(new { message = "Data from database", contact });
        }


        /// <summary>
        /// Adds a new contact to the address book.
        /// </summary>
        /// <param name="contact">The contact details to add.</param>
        /// <returns>The newly added contact.</returns>
        [HttpPost]
        public IActionResult AddContact([FromBody] AddressBookModel contact)
        {
            if (contact == null)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid contact data",
                    Data = null
                });
            }

            var addedContact = _addressBookService.AddContact(contact);

            if (addedContact == null)
            {
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Failed to add contact",
                    Data = null
                });
            }

            return Ok(new ResponseModel<AddressBookModel>
            {
                Success = true,
                Message = "Contact added successfully",
                Data = new List<AddressBookModel> { addedContact }
            });
        }

        /// <summary>
        /// Updates an existing contact by ID.
        /// </summary>
        /// <param name="id">The ID of the contact to update.</param>
        /// <param name="contact">The updated contact details.</param>
        /// <returns>The updated contact details if successful; otherwise, NotFound.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, [FromBody] AddressBookModel contact)
        {
            var result = _addressBookService.UpdateContact(id, contact);
            if (!result)
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Contact not found",
                    Data = null
                });
            }

            var updatedContact = _addressBookService.GetContactById(id);

            return Ok(new ResponseModel<AddressBookModel>
            {
                Success = true,
                Message = "Contact updated successfully",
                Data = new List<AddressBookModel> { updatedContact }
            });
        }

        /// <summary>
        /// Deletes a contact by ID.
        /// </summary>
        /// <param name="id">The ID of the contact to delete.</param>
        /// <returns>A confirmation message if successful; otherwise, NotFound.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            var result = _addressBookService.DeleteContact(id);
            if (!result)
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Contact not found",
                    Data = null
                });
            }
            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Contact deleted successfully",
                Data = null
            });
        }
    }
}