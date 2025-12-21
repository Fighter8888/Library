using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(LibraryDbContext context) : base(context)
    {
    }
}

