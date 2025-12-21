using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Book>> GetAllWithDetailsAsync();
    Task<IEnumerable<Book>> SearchAsync(string? title, string? author, string? isbn, Guid? categoryId);
}

