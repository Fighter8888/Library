using Library.Application.DTOs.Fines;
using Library.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;

    public FinesController(IFineService fineService)
    {
        _fineService = fineService;
    }

    [HttpGet("my-fines")]
    public async Task<ActionResult<List<FineDto>>> GetMyFines()
    {
        var accountId = GetCurrentAccountId();
        var fines = await _fineService.GetMyFinesAsync(accountId);
        return Ok(fines);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<FineDto>>> GetAllFines()
    {
        var fines = await _fineService.GetAllFinesAsync();
        return Ok(fines);
    }

    [HttpPost("{fineId}/pay")]
    public async Task<ActionResult> PayFine(Guid fineId)
    {
        var result = await _fineService.PayFineAsync(fineId);
        if (!result)
        {
            return BadRequest("Unable to pay fine");
        }

        return Ok();
    }

    [HttpPost("calculate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CalculateFines()
    {
        await _fineService.CalculateFinesForOverdueBorrowsAsync();
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

