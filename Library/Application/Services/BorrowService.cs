using Library.Application.DTOs.Borrows;
using Library.Application.Repositories;
using Library.Domain.Entities;
using AvailableBookStatus = Library.Domain.Entities.AvailableBookStatus;

namespace Library.Application.Services;

public class BorrowService : IBorrowService
{
    private readonly IUnitOfWork _unitOfWork;

    public BorrowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> BorrowBookAsync(Guid accountId, BorrowBookRequest request)
    {
        var availableBook = await _unitOfWork.AvailableBooks.GetByIdWithBookAsync(request.AvailableBookId);

        if (availableBook == null || availableBook.Status != AvailableBookStatus.Available)
        {
            throw new InvalidOperationException("Book is not available for borrowing");
        }

        var borrow = new Borrow
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            AvailableBookId = request.AvailableBookId,
            BorrowedAtUtc = DateTime.UtcNow,
            DueAtUtc = DateTime.UtcNow.AddDays(request.DaysToBorrow)
        };

        availableBook.Status = AvailableBookStatus.Borrowed;

        _unitOfWork.Borrows.Add(borrow);
        await _unitOfWork.SaveChangesAsync();

        return borrow.Id;
    }

    public async Task<bool> ReturnBookAsync(Guid accountId, ReturnBookRequest request)
    {
        var borrow = await _unitOfWork.Borrows.FirstOrDefaultAsync(
            b => b.Id == request.BorrowId && b.AccountId == accountId);

        if (borrow == null || borrow.ReturnedAtUtc.HasValue)
        {
            return false;
        }

        var availableBook = await _unitOfWork.AvailableBooks.GetByIdAsync(borrow.AvailableBookId);
        if (availableBook == null)
        {
            return false;
        }

        borrow.ReturnedAtUtc = DateTime.UtcNow;
        availableBook.Status = AvailableBookStatus.Available;

        _unitOfWork.Borrows.Update(borrow);
        _unitOfWork.AvailableBooks.Update(availableBook);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<BorrowDto>> GetMyBorrowsAsync(Guid accountId)
    {
        var now = DateTime.UtcNow;
        var borrows = await _unitOfWork.Borrows.GetByAccountIdAsync(accountId);
        
        return borrows.Select(b => new BorrowDto
        {
            Id = b.Id,
            BookTitle = b.AvailableBook.Book.Title,
            InventoryCode = b.AvailableBook.InventoryCode,
            BorrowedAtUtc = b.BorrowedAtUtc,
            DueAtUtc = b.DueAtUtc,
            ReturnedAtUtc = b.ReturnedAtUtc,
            IsOverdue = !b.ReturnedAtUtc.HasValue && b.DueAtUtc < now
        }).ToList();
    }
}
