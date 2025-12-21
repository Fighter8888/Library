using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AvailableBookRepository : Repository<AvailableBook>, IAvailableBookRepository
{
    public AvailableBookRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<AvailableBook?> GetByIdWithBookAsync(Guid id)
    {
        return await _dbSet
            .Include(ab => ab.Book)
            .FirstOrDefaultAsync(ab => ab.Id == id);
    }

    public async Task<bool> InventoryCodeExistsAsync(string inventoryCode)
    {
        return await _dbSet.AnyAsync(ab => ab.InventoryCode == inventoryCode);
    }
}

