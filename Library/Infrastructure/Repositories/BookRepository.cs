using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Copies)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchAsync(string? title, string? author, string? isbn, Guid? categoryId)
    {
        var query = _dbSet
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Copies)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(b => b.BookAuthors.Any(ba =>
                ba.Author.FirstName.Contains(author) || ba.Author.LastName.Contains(author)));
        }

        if (!string.IsNullOrWhiteSpace(isbn))
        {
            query = query.Where(b => b.Isbn != null && b.Isbn.Contains(isbn));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId.Value));
        }

        return await query.ToListAsync();
    }
}

