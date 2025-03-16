using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

var builder = WebApplication.CreateBuilder(args);

// ? Ensure Configuration is Correct
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException("Connection string is missing. Check appsettings.json!");
}

// ? Register DbContext with Connection String

builder.Services.AddDbContext<AddressBookDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();