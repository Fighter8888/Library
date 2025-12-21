using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IAvailableBookRepository : IRepository<AvailableBook>
{
    Task<AvailableBook?> GetByIdWithBookAsync(Guid id);
    Task<bool> InventoryCodeExistsAsync(string inventoryCode);
}

