namespace Library.Domain.Entities;

public class Fine : BaseEntity
{
    public Guid BorrowId { get; set; }
    public Borrow Borrow { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public bool IsPaid => PaidAtUtc.HasValue;

    public string? Notes { get; set; }
}

