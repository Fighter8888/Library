using Library.Application.Repositories;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Library.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(LibraryDbContext context)
    {
        _context = context;
        People = new PersonRepository(context);
        Accounts = new AccountRepository(context);
        Books = new BookRepository(context);
        Authors = new AuthorRepository(context);
        Publishers = new PublisherRepository(context);
        AvailableBooks = new AvailableBookRepository(context);
        Borrows = new BorrowRepository(context);
        Fines = new FineRepository(context);
        Roles = new RoleRepository(context);
    }

    public IPersonRepository People { get; }
    public IAccountRepository Accounts { get; }
    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }
    public IPublisherRepository Publishers { get; }
    public IAvailableBookRepository AvailableBooks { get; }
    public IBorrowRepository Borrows { get; }
    public IFineRepository Fines { get; }
    public IRoleRepository Roles { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

