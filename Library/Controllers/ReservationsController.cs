using Library.Application.DTOs.Reservations;
using Library.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateReservation([FromBody] CreateReservationRequest request)
    {
        var accountId = GetCurrentAccountId();
        try
        {
            var reservationId = await _reservationService.CreateReservationAsync(accountId, request);
            return CreatedAtAction(nameof(GetMyReservations), new { }, reservationId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{reservationId}/cancel")]
    public async Task<ActionResult> CancelReservation(Guid reservationId)
    {
        var accountId = GetCurrentAccountId();
        var result = await _reservationService.CancelReservationAsync(accountId, reservationId);
        if (!result)
        {
            return BadRequest("Unable to cancel reservation");
        }

        return Ok();
    }

    [HttpGet("my-reservations")]
    public async Task<ActionResult<List<ReservationDto>>> GetMyReservations()
    {
        var accountId = GetCurrentAccountId();
        var reservations = await _reservationService.GetMyReservationsAsync(accountId);
        return Ok(reservations);
    }

    [HttpPost("{reservationId}/convert-to-borrow")]
    public async Task<ActionResult<Guid>> ConvertToBorrow(Guid reservationId, [FromQuery] int? daysToBorrow = 14)
    {
        var accountId = GetCurrentAccountId();
        var result = await _reservationService.ConvertReservationToBorrowAsync(accountId, reservationId, daysToBorrow ?? 14);
        if (!result)
        {
            return BadRequest("Unable to convert reservation to borrow");
        }

        return Ok();
    }

    [HttpPost("expire-old")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ExpireOldReservations()
    {
        await _reservationService.ExpireOldReservationsAsync();
        return Ok();
    }

    private Guid GetCurrentAccountId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var accountId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }

        return accountId;
    }
}

