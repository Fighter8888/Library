using Library.Application.DTOs.Fines;

namespace Library.Application.Services;

public interface IFineService
{
    Task<List<FineDto>> GetMyFinesAsync(Guid accountId);
    Task<List<FineDto>> GetAllFinesAsync();
    Task<bool> PayFineAsync(Guid fineId);
    Task CalculateFinesForOverdueBorrowsAsync();
}

