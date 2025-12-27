using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Include(r => r.AvailableBook)
                .ThenInclude(ab => ab.Book)
            .Where(r => r.AccountId == accountId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.AvailableBook)
                .ThenInclude(ab => ab.Book)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(r => r.ExpiresAtUtc.HasValue && 
                       r.ExpiresAtUtc < now && 
                       r.CancelledAtUtc == null)
            .ToListAsync();
    }

    public async Task<bool> HasActiveReservationAsync(Guid accountId, Guid availableBookId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet.AnyAsync(r =>
            r.AccountId == accountId &&
            r.AvailableBookId == availableBookId &&
            r.CancelledAtUtc == null &&
            (r.ExpiresAtUtc == null || r.ExpiresAtUtc > now));
    }
}

