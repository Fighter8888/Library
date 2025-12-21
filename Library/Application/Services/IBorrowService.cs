using Library.Application.DTOs.Borrows;

namespace Library.Application.Services;

public interface IBorrowService
{
    Task<Guid> BorrowBookAsync(Guid accountId, BorrowBookRequest request);
    Task<bool> ReturnBookAsync(Guid accountId, ReturnBookRequest request);
    Task<List<BorrowDto>> GetMyBorrowsAsync(Guid accountId);
}

public class BorrowDto
{
    public Guid Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string InventoryCode { get; set; } = string.Empty;
    public DateTime BorrowedAtUtc { get; set; }
    public DateTime DueAtUtc { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }
    public bool IsOverdue { get; set; }
}

