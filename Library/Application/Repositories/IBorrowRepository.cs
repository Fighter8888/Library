using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IBorrowRepository : IRepository<Borrow>
{
    Task<IEnumerable<Borrow>> GetByAccountIdAsync(Guid accountId);
    Task<Borrow?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync();
}

