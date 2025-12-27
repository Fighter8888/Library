namespace Library.Application.DTOs.Reservations;

public class CreateReservationRequest
{
    public Guid AvailableBookId { get; set; }
    public int? ExpirationDays { get; set; } = 7; // Default 7 days
}

