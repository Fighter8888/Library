using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(LibraryDbContext context) : base(context)
    {
    }
}

