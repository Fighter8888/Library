using Library.Application.DTOs.Books;
using Library.Application.Repositories;
using Library.Domain.Entities;
using AvailableBookStatus = Library.Domain.Entities.AvailableBookStatus;

namespace Library.Application.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        var books = await _unitOfWork.Books.GetAllWithDetailsAsync();
        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Isbn = b.Isbn,
            PublicationYear = b.PublicationYear,
            PublisherName = b.Publisher.Name,
            Authors = b.BookAuthors.Select(ba => ba.Author.FullName).ToList(),
            Categories = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
            AvailableCopies = b.Copies.Count(c => c.Status == AvailableBookStatus.Available),
            TotalCopies = b.Copies.Count
        }).ToList();
    }

    public async Task<List<BookDto>> SearchBooksAsync(string? title, string? author, string? isbn, Guid? categoryId)
    {
        var books = await _unitOfWork.Books.SearchAsync(title, author, isbn, categoryId);
        return books.Select(b => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Isbn = b.Isbn,
            PublicationYear = b.PublicationYear,
            PublisherName = b.Publisher.Name,
            Authors = b.BookAuthors.Select(ba => ba.Author.FullName).ToList(),
            Categories = b.BookCategories.Select(bc => bc.Category.Name).ToList(),
            AvailableCopies = b.Copies.Count(c => c.Status == AvailableBookStatus.Available),
            TotalCopies = b.Copies.Count
        }).ToList();
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdWithDetailsAsync(id);
        if (book == null)
        {
            return null;
        }

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublicationYear = book.PublicationYear,
            PublisherName = book.Publisher.Name,
            Authors = book.BookAuthors.Select(ba => ba.Author.FullName).ToList(),
            Categories = book.BookCategories.Select(bc => bc.Category.Name).ToList(),
            AvailableCopies = book.Copies.Count(c => c.Status == AvailableBookStatus.Available),
            TotalCopies = book.Copies.Count
        };
    }

    public async Task<Guid> CreateBookAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Isbn = request.Isbn,
            PublicationYear = request.PublicationYear,
            PublisherId = request.PublisherId
        };

        foreach (var authorId in request.AuthorIds)
        {
            book.BookAuthors.Add(new BookAuthor
            {
                BookId = book.Id,
                AuthorId = authorId
            });
        }

        foreach (var categoryId in request.CategoryIds)
        {
            book.BookCategories.Add(new BookCategory
            {
                BookId = book.Id,
                CategoryId = categoryId
            });
        }

        _unitOfWork.Books.Add(book);
        await _unitOfWork.SaveChangesAsync();

        return book.Id;
    }

    public async Task<Guid> CreateAuthorAsync(CreateAuthorRequest request)
    {
        var author = new Author
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Biography = request.Biography
        };

        _unitOfWork.Authors.Add(author);
        await _unitOfWork.SaveChangesAsync();

        return author.Id;
    }

    public async Task<List<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _unitOfWork.Authors.GetAllAsync();
        return authors.Select(a => new AuthorDto
        {
            Id = a.Id,
            FirstName = a.FirstName,
            LastName = a.LastName,
            FullName = a.FullName,
            Biography = a.Biography
        }).ToList();
    }

    public async Task<Guid> CreatePublisherAsync(CreatePublisherRequest request)
    {
        var publisher = new Publisher
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        _unitOfWork.Publishers.Add(publisher);
        await _unitOfWork.SaveChangesAsync();

        return publisher.Id;
    }

    public async Task<List<PublisherDto>> GetAllPublishersAsync()
    {
        var publishers = await _unitOfWork.Publishers.GetAllAsync();
        return publishers.Select(p => new PublisherDto
        {
            Id = p.Id,
            Name = p.Name
        }).ToList();
    }

    public async Task<Guid> AddInventoryAsync(AddInventoryRequest request)
    {
        var availableBook = new AvailableBook
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            InventoryCode = request.InventoryCode,
            Status = AvailableBookStatus.Available
        };

        _unitOfWork.AvailableBooks.Add(availableBook);
        await _unitOfWork.SaveChangesAsync();

        return availableBook.Id;
    }
}
