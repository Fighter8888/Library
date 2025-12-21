using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}

