using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using System.Collections.Generic;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace RepositoryLayer
{
    /// <summary>
    /// Represents the database context for the Address Book application.
    /// </summary>
    public class AddressBookDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressBookDbContext"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        public AddressBookDbContext(DbContextOptions<AddressBookDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the DbSet for storing user address book entries.
        /// </summary>
        public DbSet<AddressBookEntity> AddressBookEntries { get; set; }
    }
}