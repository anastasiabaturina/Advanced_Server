using FluentValidation;
using Server_Ad_Baturina.Models.Requests;

namespace Server_Ad_Baturina.Validations;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }
}