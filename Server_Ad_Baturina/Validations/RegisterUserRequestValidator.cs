using FluentValidation;
using Server_Ad_Baturina.Models.Requests;

namespace Server_Ad_Baturina.Validations;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{ 
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .MaximumLength(25).WithMessage("Name can't be more than 25 characters long"); ;

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .MaximumLength(100).WithMessage("Email can't be more than 100 characters long")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required")
            .MaximumLength(25).WithMessage("Role can't be more than 25 characters long");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}