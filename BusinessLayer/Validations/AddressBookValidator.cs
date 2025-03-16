using FluentValidation;
using ModelLayer;
using ModelLayer.DTO;

namespace BusinessLayer.Validations
{
    /// <summary>
    /// Validator for AddressBookEntryDto to enforce data validation rules.
    /// </summary>
    public class AddressBookValidator : AbstractValidator<AddressBookDTO>
    {
        public AddressBookValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches("^[0-9]{10}$").WithMessage("Phone number must be 10 digits");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        }
    }
}