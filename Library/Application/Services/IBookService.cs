using Library.Application.DTOs.Books;

namespace Library.Application.Services;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<List<BookDto>> SearchBooksAsync(string? title, string? author, string? isbn, Guid? categoryId);
    Task<BookDto?> GetBookByIdAsync(Guid id);
    Task<Guid> CreateBookAsync(CreateBookRequest request);
    Task<Guid> CreateAuthorAsync(CreateAuthorRequest request);
    Task<List<AuthorDto>> GetAllAuthorsAsync();
    Task<Guid> CreatePublisherAsync(CreatePublisherRequest request);
    Task<List<PublisherDto>> GetAllPublishersAsync();
    Task<Guid> AddInventoryAsync(AddInventoryRequest request);
}

public class AuthorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Biography { get; set; }
}

public class PublisherDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

