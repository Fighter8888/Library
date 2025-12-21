using Library.Application.DTOs.Fines;
using Library.Application.Repositories;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class FineService : IFineService
{
    private readonly IUnitOfWork _unitOfWork;
    private const decimal DailyFineRate = 0.50m; // $0.50 per day

    public FineService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FineDto>> GetMyFinesAsync(Guid accountId)
    {
        var fines = await _unitOfWork.Fines.GetByAccountIdAsync(accountId);
        return fines.Select(f => new FineDto
        {
            Id = f.Id,
            BorrowId = f.BorrowId,
            BookTitle = f.Borrow.AvailableBook.Book.Title,
            Amount = f.Amount,
            DueDate = f.DueDate,
            PaidAtUtc = f.PaidAtUtc,
            IsPaid = f.IsPaid,
            Notes = f.Notes
        }).ToList();
    }

    public async Task<List<FineDto>> GetAllFinesAsync()
    {
        var fines = await _unitOfWork.Fines.GetAllWithDetailsAsync();
        return fines.Select(f => new FineDto
        {
            Id = f.Id,
            BorrowId = f.BorrowId,
            BookTitle = f.Borrow.AvailableBook.Book.Title,
            Amount = f.Amount,
            DueDate = f.DueDate,
            PaidAtUtc = f.PaidAtUtc,
            IsPaid = f.IsPaid,
            Notes = f.Notes
        }).ToList();
    }

    public async Task<bool> PayFineAsync(Guid fineId)
    {
        var fine = await _unitOfWork.Fines.GetByIdAsync(fineId);
        if (fine == null || fine.IsPaid)
        {
            return false;
        }

        fine.PaidAtUtc = DateTime.UtcNow;
        _unitOfWork.Fines.Update(fine);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task CalculateFinesForOverdueBorrowsAsync()
    {
        var overdueBorrows = await _unitOfWork.Borrows.GetOverdueBorrowsAsync();
        var now = DateTime.UtcNow;

        foreach (var borrow in overdueBorrows)
        {
            var existingFine = await _unitOfWork.Fines.FirstOrDefaultAsync(
                f => f.BorrowId == borrow.Id && !f.IsPaid);

            if (existingFine == null)
            {
                var daysOverdue = (now - borrow.DueAtUtc).Days;
                var fineAmount = daysOverdue * DailyFineRate;

                var fine = new Fine
                {
                    Id = Guid.NewGuid(),
                    BorrowId = borrow.Id,
                    Amount = fineAmount,
                    DueDate = borrow.DueAtUtc,
                    Notes = $"Fine for {daysOverdue} days overdue"
                };

                _unitOfWork.Fines.Add(fine);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
