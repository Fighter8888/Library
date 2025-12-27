namespace Library.Application.DTOs.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BookCount { get; set; }
}

