namespace Library.Application.DTOs.Reservations;

public class ReservationDto
{
    public Guid Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string InventoryCode { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}

