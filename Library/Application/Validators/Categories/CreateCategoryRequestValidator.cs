using FluentValidation;
using Library.Application.DTOs.Categories;

namespace Library.Application.Validators.Categories;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters")
            .Matches("^[a-zA-Z0-9\\s-]+$").WithMessage("Category name contains invalid characters");
    }
}

