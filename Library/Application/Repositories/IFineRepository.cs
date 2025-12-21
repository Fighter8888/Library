using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IFineRepository : IRepository<Fine>
{
    Task<IEnumerable<Fine>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<Fine>> GetAllWithDetailsAsync();
    Task<Fine?> GetByIdWithDetailsAsync(Guid id);
}

