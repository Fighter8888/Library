namespace Library.Domain.Entities;

public class AccountRole
{
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}


