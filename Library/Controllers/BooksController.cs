using Library.Application.DTOs.Books;
using Library.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<BookDto>>> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<List<BookDto>>> SearchBooks(
        [FromQuery] string? title,
        [FromQuery] string? author,
        [FromQuery] string? isbn,
        [FromQuery] Guid? categoryId)
    {
        var books = await _bookService.SearchBooksAsync(title, author, isbn, categoryId);
        return Ok(books);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<BookDto>> GetBookById(Guid id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateBook([FromBody] CreateBookRequest request)
    {
        var bookId = await _bookService.CreateBookAsync(request);
        return CreatedAtAction(nameof(GetBookById), new { id = bookId }, bookId);
    }

    [HttpPost("authors")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateAuthor([FromBody] CreateAuthorRequest request)
    {
        var authorId = await _bookService.CreateAuthorAsync(request);
        return Ok(authorId);
    }

    [HttpGet("authors")]
    [AllowAnonymous]
    public async Task<ActionResult<List<AuthorDto>>> GetAllAuthors()
    {
        var authors = await _bookService.GetAllAuthorsAsync();
        return Ok(authors);
    }

    [HttpPost("publishers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreatePublisher([FromBody] CreatePublisherRequest request)
    {
        var publisherId = await _bookService.CreatePublisherAsync(request);
        return Ok(publisherId);
    }

    [HttpGet("publishers")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PublisherDto>>> GetAllPublishers()
    {
        var publishers = await _bookService.GetAllPublishersAsync();
        return Ok(publishers);
    }

    [HttpPost("inventory")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> AddInventory([FromBody] AddInventoryRequest request)
    {
        var inventoryId = await _bookService.AddInventoryAsync(request);
        return Ok(inventoryId);
    }
}

