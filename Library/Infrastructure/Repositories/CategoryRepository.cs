using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(c => c.Name == name);
    }

    public async Task<bool> NameExistsAsync(string name, Guid excludeId)
    {
        return await _dbSet.AnyAsync(c => c.Name == name && c.Id != excludeId);
    }
}

