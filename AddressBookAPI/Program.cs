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
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using BusinessLayer.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("Jwt"));
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Implementing redis cache
builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "SampleInstance";
});

// Register RedisCacheService
builder.Services.AddSingleton<RedisCacheService>();

// ? Ensure Configuration is Correct
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException("Connection string is missing. Check appsettings.json!");
}

// ? Register DbContext with Connection String

builder.Services.AddDbContext<AddressBookDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Start RabbitMQ Consumer
var userConsumer = new RabbitMQConsumer("user_registered");
var contactConsumer = new RabbitMQConsumer("contact_added");

Task.Run(() => userConsumer.StartListening());
Task.Run(() => contactConsumer.StartListening());


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
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Address Book API",
        Version = "v1",
        Description = "API for managing contacts in the Address Book application."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Address Book API V1");
    });
}



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();