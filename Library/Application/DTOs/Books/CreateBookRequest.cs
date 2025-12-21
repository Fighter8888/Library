namespace Library.Application.DTOs.Books;

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Isbn { get; set; }
    public int? PublicationYear { get; set; }
    public Guid PublisherId { get; set; }
    public List<Guid> AuthorIds { get; set; } = new();
    public List<Guid> CategoryIds { get; set; } = new();
}

