using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByAccountIdAsync(Guid accountId);
    Task<Reservation?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();
    Task<bool> HasActiveReservationAsync(Guid accountId, Guid availableBookId);
}

