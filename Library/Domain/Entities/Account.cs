namespace Library.Domain.Entities;

public class Account : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;

    public ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
}


