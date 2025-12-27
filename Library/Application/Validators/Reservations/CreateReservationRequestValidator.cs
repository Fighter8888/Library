using FluentValidation;
using Library.Application.DTOs.Reservations;

namespace Library.Application.Validators.Reservations;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.AvailableBookId)
            .NotEmpty().WithMessage("Available book ID is required");

        RuleFor(x => x.ExpirationDays)
            .InclusiveBetween(1, 30).WithMessage("Expiration days must be between 1 and 30")
            .When(x => x.ExpirationDays.HasValue);
    }
}

