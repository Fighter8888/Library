using FluentValidation;
using Library.Application.DTOs.Auth;

namespace Library.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

