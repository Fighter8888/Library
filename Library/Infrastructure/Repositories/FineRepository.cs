using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class FineRepository : Repository<Fine>, IFineRepository
{
    public FineRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Fine>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Include(f => f.Borrow)
                .ThenInclude(b => b.AvailableBook)
                    .ThenInclude(ab => ab.Book)
            .Where(f => f.Borrow.AccountId == accountId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fine>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(f => f.Borrow)
                .ThenInclude(b => b.AvailableBook)
                    .ThenInclude(ab => ab.Book)
            .ToListAsync();
    }

    public async Task<Fine?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.Borrow)
                .ThenInclude(b => b.AvailableBook)
                    .ThenInclude(ab => ab.Book)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
}

