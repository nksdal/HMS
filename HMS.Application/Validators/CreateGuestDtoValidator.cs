using FluentValidation;
using HMS.Application.DTOs.Guests;

namespace HMS.Application.Validators;

public class CreateGuestDtoValidator : AbstractValidator<CreateGuestDto>
{
    public CreateGuestDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PersonalNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
    }
}
