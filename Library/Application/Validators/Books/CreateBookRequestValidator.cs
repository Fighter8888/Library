using FluentValidation;
using Library.Application.DTOs.Books;

namespace Library.Application.Validators.Books;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Isbn)
            .MaximumLength(20).WithMessage("ISBN must not exceed 20 characters")
            .Matches("^[0-9-]+$").WithMessage("ISBN can only contain numbers and hyphens")
            .When(x => !string.IsNullOrEmpty(x.Isbn));

        RuleFor(x => x.PublicationYear)
            .InclusiveBetween(1000, DateTime.Now.Year + 1)
            .WithMessage($"Publication year must be between 1000 and {DateTime.Now.Year + 1}")
            .When(x => x.PublicationYear.HasValue);

        RuleFor(x => x.PublisherId)
            .NotEmpty().WithMessage("Publisher is required");

        RuleFor(x => x.AuthorIds)
            .NotEmpty().WithMessage("At least one author is required");

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("At least one category is required");
    }
}

