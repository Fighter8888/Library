namespace Library.Domain.Entities;

public class Publisher : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}



