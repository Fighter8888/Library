using Library.Application.DTOs.Reservations;
using Library.Application.Repositories;
using Library.Domain.Entities;
using AvailableBookStatus = Library.Domain.Entities.AvailableBookStatus;

namespace Library.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int DefaultExpirationDays = 7;
    private const int MaxExpirationDays = 30; // Security: Limit reservation duration

    public ReservationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateReservationAsync(Guid accountId, CreateReservationRequest request)
    {
        // Security: Validate available book exists and is available
        var availableBook = await _unitOfWork.AvailableBooks.GetByIdWithBookAsync(request.AvailableBookId);
        if (availableBook == null)
        {
            throw new InvalidOperationException("Book copy not found");
        }

        if (availableBook.Status != AvailableBookStatus.Available)
        {
            throw new InvalidOperationException("Book is not available for reservation");
        }

        // Security: Prevent duplicate active reservations
        if (await _unitOfWork.Reservations.HasActiveReservationAsync(accountId, request.AvailableBookId))
        {
            throw new InvalidOperationException("You already have an active reservation for this book");
        }

        // Security: Validate expiration days
        var expirationDays = request.ExpirationDays ?? DefaultExpirationDays;
        if (expirationDays <= 0 || expirationDays > MaxExpirationDays)
        {
            throw new ArgumentException($"Expiration days must be between 1 and {MaxExpirationDays}");
        }

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            AvailableBookId = request.AvailableBookId,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays)
        };

        availableBook.Status = AvailableBookStatus.Reserved;

        _unitOfWork.Reservations.Add(reservation);
        await _unitOfWork.SaveChangesAsync();

        return reservation.Id;
    }

    public async Task<bool> CancelReservationAsync(Guid accountId, Guid reservationId)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdWithDetailsAsync(reservationId);
        
        // Security: Ensure user can only cancel their own reservations
        if (reservation == null || reservation.AccountId != accountId)
        {
            return false;
        }

        if (reservation.CancelledAtUtc.HasValue)
        {
            return false; // Already cancelled
        }

        reservation.CancelledAtUtc = DateTime.UtcNow;
        
        // Release the book back to available status
        var availableBook = await _unitOfWork.AvailableBooks.GetByIdAsync(reservation.AvailableBookId);
        if (availableBook != null)
        {
            availableBook.Status = AvailableBookStatus.Available;
            _unitOfWork.AvailableBooks.Update(availableBook);
        }

        _unitOfWork.Reservations.Update(reservation);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<ReservationDto>> GetMyReservationsAsync(Guid accountId)
    {
        var now = DateTime.UtcNow;
        var reservations = await _unitOfWork.Reservations.GetByAccountIdAsync(accountId);
        
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            BookTitle = r.AvailableBook.Book.Title,
            InventoryCode = r.AvailableBook.InventoryCode,
            CreatedAtUtc = r.CreatedAtUtc,
            ExpiresAtUtc = r.ExpiresAtUtc,
            CancelledAtUtc = r.CancelledAtUtc,
            IsActive = r.CancelledAtUtc == null && (r.ExpiresAtUtc == null || r.ExpiresAtUtc > now),
            IsExpired = r.ExpiresAtUtc.HasValue && r.ExpiresAtUtc < now && r.CancelledAtUtc == null
        }).ToList();
    }

    public async Task ExpireOldReservationsAsync()
    {
        var expiredReservations = await _unitOfWork.Reservations.GetExpiredReservationsAsync();
        var now = DateTime.UtcNow;

        foreach (var reservation in expiredReservations)
        {
            reservation.CancelledAtUtc = now;
            
            // Release the book back to available
            var availableBook = await _unitOfWork.AvailableBooks.GetByIdAsync(reservation.AvailableBookId);
            if (availableBook != null)
            {
                availableBook.Status = AvailableBookStatus.Available;
                _unitOfWork.AvailableBooks.Update(availableBook);
            }

            _unitOfWork.Reservations.Update(reservation);
        }

        if (expiredReservations.Any())
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ConvertReservationToBorrowAsync(Guid accountId, Guid reservationId, int daysToBorrow = 14)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdWithDetailsAsync(reservationId);
        
        // Security: Ensure user can only convert their own reservations
        if (reservation == null || reservation.AccountId != accountId)
        {
            return false;
        }

        if (reservation.CancelledAtUtc.HasValue)
        {
            return false; // Reservation is cancelled
        }

        // Check if reservation is still valid
        if (reservation.ExpiresAtUtc.HasValue && reservation.ExpiresAtUtc < DateTime.UtcNow)
        {
            return false; // Reservation expired
        }

        // Create borrow record
        var borrow = new Borrow
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            AvailableBookId = reservation.AvailableBookId,
            BorrowedAtUtc = DateTime.UtcNow,
            DueAtUtc = DateTime.UtcNow.AddDays(daysToBorrow)
        };

        // Update book status
        var availableBook = await _unitOfWork.AvailableBooks.GetByIdAsync(reservation.AvailableBookId);
        if (availableBook == null)
        {
            return false;
        }

        availableBook.Status = AvailableBookStatus.Borrowed;

        // Cancel reservation
        reservation.CancelledAtUtc = DateTime.UtcNow;

        _unitOfWork.Borrows.Add(borrow);
        _unitOfWork.AvailableBooks.Update(availableBook);
        _unitOfWork.Reservations.Update(reservation);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

