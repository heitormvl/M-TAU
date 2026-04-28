using FluentValidation;
using M_TAU.Application.Dtos.Identity;

namespace M_TAU.Application.Validators.Identity;

public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(x => x.Email)
            .Must(email => email == email.Trim())
            .WithMessage("Email must not contain leading or trailing whitespace.");

        RuleFor(x => x.Password)
            .Must(password => password.Any(char.IsDigit))
            .WithMessage("Password must contain at least one digit.")
            .Must(password => password.Any(char.IsLetter))
            .WithMessage("Password must contain at least one letter.");

        RuleFor(x => x)
            .Must(dto => dto.Password != dto.Name)
            .WithMessage("Password must not be the same as the name.");
    }
}
