using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsAsync(string name, Guid excludeId);
}

