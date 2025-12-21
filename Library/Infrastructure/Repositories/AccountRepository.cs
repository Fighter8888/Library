using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<Account?> GetByUsernameWithRolesAsync(string username)
    {
        return await _dbSet
            .Include(a => a.AccountRoles)
                .ThenInclude(ar => ar.Role)
            .FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(a => a.Username == username);
    }
}

