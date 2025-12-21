using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByUsernameAsync(string username);
    Task<Account?> GetByUsernameWithRolesAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
}

