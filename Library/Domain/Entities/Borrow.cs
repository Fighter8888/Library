namespace Library.Domain.Entities;

public class Borrow : BaseEntity
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid AvailableBookId { get; set; }
    public AvailableBook AvailableBook { get; set; } = null!;

    public DateTime BorrowedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime DueAtUtc { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }
}



