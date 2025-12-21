namespace Library.Domain.Entities;

/// <summary>
/// Physical copy of a book (sometimes called "BookItem").
/// </summary>
public class AvailableBook : BaseEntity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    /// <summary>
    /// Inventory identifier (barcode / RFID / internal code).
    /// </summary>
    public string InventoryCode { get; set; } = string.Empty;

    public AvailableBookStatus Status { get; set; } = AvailableBookStatus.Available;

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}


