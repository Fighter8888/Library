namespace Library.Application.DTOs.Borrows;

public class BorrowBookRequest
{
    public Guid AvailableBookId { get; set; }
    public int DaysToBorrow { get; set; } = 14; // Default 2 weeks
}

