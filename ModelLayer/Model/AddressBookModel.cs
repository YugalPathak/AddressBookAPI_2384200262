using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Model
{
    /// <summary>
    /// Represents an entry in the address book.
    /// </summary>
    public class AddressBookModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}