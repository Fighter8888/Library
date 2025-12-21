using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BorrowRepository : Repository<Borrow>, IBorrowRepository
{
    public BorrowRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Borrow>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Include(b => b.AvailableBook)
                .ThenInclude(ab => ab.Book)
            .Where(b => b.AccountId == accountId)
            .ToListAsync();
    }

    public async Task<Borrow?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.AvailableBook)
                .ThenInclude(ab => ab.Book)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(b => b.AvailableBook)
            .Where(b => !b.ReturnedAtUtc.HasValue && b.DueAtUtc < now)
            .ToListAsync();
    }
}

