using FluentValidation;
using Server_Ad_Baturina.Models.Requests;

namespace Server_Ad_Baturina.Validations;

public class NewsRequestValidator : AbstractValidator<NewsRequest>
{
    public NewsRequestValidator()
    {
        RuleFor(news => news.Description)
           .MaximumLength(160).WithMessage("Description must not exceed 500 characters.");

        RuleFor(news => news.Tags)
            .ForEach(tag => tag
                .MaximumLength(160).WithMessage("Each tag must not exceed 160 characters.")
            );

        RuleFor(news => news.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(5, 130).WithMessage("Title must be between 5 and 130 characters.");
    }
}