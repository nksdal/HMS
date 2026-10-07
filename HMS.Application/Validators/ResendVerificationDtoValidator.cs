using FluentValidation;
using HMS.Application.DTOs.Auth;

namespace HMS.Application.Validators;

public class ResendVerificationDtoValidator : AbstractValidator<ResendVerificationDto>
{
    public ResendVerificationDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
