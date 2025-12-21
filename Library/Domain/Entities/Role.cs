namespace Library.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
}


