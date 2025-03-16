using BusinessLayer.Interface;
using BusinessLayer.Mappings;
using BusinessLayer.Service;
using BusinessLayer.Validations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ModelLayer.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("Jwt"));
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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
// Dependency Injection
builder.Services.AddScoped<IAddressBookRL, AddressBookRL>();
builder.Services.AddScoped<IAddressBookBL, AddressBookBL>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure SMTP settings from appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AddressBookMappingProfile));

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AddressBookValidator>();

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero // Token expiry time exact rakhta hai
    };
});

builder.Services.AddAuthorization(); // Authorization Enable karo

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();