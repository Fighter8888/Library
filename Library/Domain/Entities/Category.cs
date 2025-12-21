namespace Library.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}


