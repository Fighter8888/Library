using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Repositories;

public class PublisherRepository : Repository<Publisher>, IPublisherRepository
{
    public PublisherRepository(LibraryDbContext context) : base(context)
    {
    }
}

