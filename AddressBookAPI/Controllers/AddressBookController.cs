using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using BusinessLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace AddressBookAPI.Controllers
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookController"/> class.
        /// </summary>
        /// <param name="addressBookService">Business layer service for address book operations.</param>
        public AddressBookController(IAddressBookBL addressBookService)
        {
            _addressBookService = addressBookService;
        }

        /// <summary>
        /// Retrieves all contacts from the address book.
        /// </summary>
        /// <returns>A list of contacts.</returns>
        [HttpGet]
        public IActionResult GetContacts()
        {
            var contacts = _addressBookService.GetAllContacts();
            return Ok(new ResponseModel<IEnumerable<AddressBookEntry>>
            {
                Success = true,
                Message = "Contacts retrieved successfully",
                Data = contacts
            });
        }

        /// <summary>
        /// Retrieves a contact by its ID.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <returns>The contact details if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        public IActionResult GetContactById(int id)
        {
            var contact = _addressBookService.GetContactById(id);
            if (contact == null)
            {
                return NotFound(new ResponseModel<string> { Success = false, Message = "Contact not found", Data = null });
            }
            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact retrieved successfully",
                Data = new List<AddressBookEntry> { contact }
            });
        }

        /// <summary>
        /// Adds a new contact to the address book.
        /// </summary>
        /// <param name="contact">The contact details to add.</param>
        /// <returns>The newly added contact.</returns>
        [HttpPost]
        public IActionResult AddContact([FromBody] AddressBookEntry contact)
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

            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact added successfully",
                Data = new List<AddressBookEntry> { addedContact }
            });
        }

        /// <summary>
        /// Updates an existing contact by ID.
        /// </summary>
        /// <param name="id">The ID of the contact to update.</param>
        /// <param name="contact">The updated contact details.</param>
        /// <returns>The updated contact details if successful; otherwise, NotFound.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, [FromBody] AddressBookEntry contact)
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

            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact updated successfully",
                Data = new List<AddressBookEntry> { updatedContact }
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