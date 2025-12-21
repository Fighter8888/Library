namespace Library.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public int? PublicationYear { get; set; }

    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<AvailableBook> Copies { get; set; } = new List<AvailableBook>();
}



