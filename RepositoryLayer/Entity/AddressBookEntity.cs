using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    /// <summary>
    /// Represents a user entity in the system.
    /// </summary>
    public class AddressBookEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public string? ResetToken { get; internal set; }
        public DateTime? ResetTokenExpiry { get; internal set; }
    }
}