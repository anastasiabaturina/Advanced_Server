using FluentValidation;
using Server_Ad_Baturina.Models.Requests;

namespace Server_Ad_Baturina.Validations;

public class PutUserRequestValidator : AbstractValidator<PutUserRequest>
{
    public PutUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
          .MaximumLength(25).WithMessage("Name can't be more than 25 characters long"); ;

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .MaximumLength(100).WithMessage("Email can't be more than 100 characters long")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required")
            .MaximumLength(25).WithMessage("Role can't be more than 25 characters long");
    }
}