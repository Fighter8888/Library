using Library.Application.DTOs.Categories;
using Library.Application.Repositories;
using Library.Domain.Entities;

namespace Library.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            BookCount = c.BookCategories.Count
        }).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            BookCount = category.BookCategories.Count
        };
    }

    public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request)
    {
        // Security: Check for duplicate names to prevent conflicts
        if (await _unitOfWork.Categories.NameExistsAsync(request.Name))
        {
            throw new InvalidOperationException("Category name already exists");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim()
        };

        _unitOfWork.Categories.Add(category);
        await _unitOfWork.SaveChangesAsync();

        return category.Id;
    }

    public async Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        // Security: Check for duplicate names excluding current category
        if (await _unitOfWork.Categories.NameExistsAsync(request.Name, id))
        {
            throw new InvalidOperationException("Category name already exists");
        }

        category.Name = request.Name.Trim();
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            return false;
        }

        // Security: Prevent deletion if category has books assigned
        if (category.BookCategories.Any())
        {
            throw new InvalidOperationException("Cannot delete category that has books assigned");
        }

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}

