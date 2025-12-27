using Library.Application.DTOs.Reservations;

namespace Library.Application.Services;

public interface IReservationService
{
    Task<Guid> CreateReservationAsync(Guid accountId, CreateReservationRequest request);
    Task<bool> CancelReservationAsync(Guid accountId, Guid reservationId);
    Task<List<ReservationDto>> GetMyReservationsAsync(Guid accountId);
    Task ExpireOldReservationsAsync();
    Task<bool> ConvertReservationToBorrowAsync(Guid accountId, Guid reservationId, int daysToBorrow = 14);
}

