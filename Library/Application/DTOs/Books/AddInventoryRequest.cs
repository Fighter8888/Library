namespace Library.Application.DTOs.Books;

public class AddInventoryRequest
{
    public Guid BookId { get; set; }
    public string InventoryCode { get; set; } = string.Empty;
}

