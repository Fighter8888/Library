namespace Library.Application.DTOs.Books;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public int? PublicationYear { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public int AvailableCopies { get; set; }
    public int TotalCopies { get; set; }
}

