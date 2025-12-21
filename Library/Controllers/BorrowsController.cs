using Library.Application.DTOs.Borrows;
using Library.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BorrowsController : ControllerBase
{
    private readonly IBorrowService _borrowService;

    public BorrowsController(IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> BorrowBook([FromBody] BorrowBookRequest request)
    {
        var accountId = GetCurrentAccountId();
        try
        {
            var borrowId = await _borrowService.BorrowBookAsync(accountId, request);
            return Ok(borrowId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("return")]
    public async Task<ActionResult> ReturnBook([FromBody] ReturnBookRequest request)
    {
        var accountId = GetCurrentAccountId();
        var result = await _borrowService.ReturnBookAsync(accountId, request);
        if (!result)
        {
            return BadRequest("Unable to return book");
        }

        return Ok();
    }

    [HttpGet("my-borrows")]
    public async Task<ActionResult<List<BorrowDto>>> GetMyBorrows()
    {
        var accountId = GetCurrentAccountId();
        var borrows = await _borrowService.GetMyBorrowsAsync(accountId);
        return Ok(borrows);
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

