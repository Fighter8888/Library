using Library.Application.DTOs.Categories;

namespace Library.Application.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<Guid> CreateCategoryAsync(CreateCategoryRequest request);
    Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(Guid id);
}

