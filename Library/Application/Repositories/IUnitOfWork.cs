namespace Library.Application.Repositories;

public interface IUnitOfWork : IDisposable
{
    IPersonRepository People { get; }
    IAccountRepository Accounts { get; }
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    IPublisherRepository Publishers { get; }
    IAvailableBookRepository AvailableBooks { get; }
    IBorrowRepository Borrows { get; }
    IFineRepository Fines { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

