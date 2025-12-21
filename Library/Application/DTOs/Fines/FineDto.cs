namespace Library.Application.DTOs.Fines;

public class FineDto
{
    public Guid Id { get; set; }
    public Guid BorrowId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public bool IsPaid { get; set; }
    public string? Notes { get; set; }
}

