using VirtualWallet.Business.DTOs.Categories;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Business.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;

    public CategoryService(ICategoryRepository categoryRepository, IUserRepository userRepository)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<CategoryResponseDto>> GetUserCategoriesAsync(int userId)
    {
        IEnumerable<Category> categories = await _categoryRepository.GetByUserIdAsync(userId);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(int userId, CategoryCreateDto categoryDto)
    {
        User user = await _userRepository.GetByIdAsync(userId)
            ?? throw new EntityNotFoundException($"User with id {userId} not found");
        
        IEnumerable<Category> existingCategories = await _categoryRepository.GetByUserIdAsync(userId);
        string categoryName = categoryDto.Name.Trim();
        
        //check for existing category
        if (existingCategories.Any(c => string.Equals(categoryName, c.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException($"You have already created a category named {categoryDto.Name}.");
        }
        
        Category category = new Category
        {
            Name =  categoryName,
            UserId = user.Id
        };

        await _categoryRepository.AddAsync(category);
        return MapToDto(category);
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(int userId, int categoryId)
    {
        Category category = await _categoryRepository.GetByIdAsync(categoryId)
            ?? throw new EntityNotFoundException($"Category with id {categoryId} not found");

        if (category.UserId != userId)
        {
            throw new UnauthorizedOperationException($"You do not own this category.");
        }
        
        return MapToDto(category);
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(int userId, int categoryId, CategoryUpdateDto categoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId)
            ?? throw new EntityNotFoundException($"Category with id {categoryId} not found");
        
        if (category.UserId != userId)
        {
            throw new UnauthorizedOperationException($"You do not own this category.");
        }
        
        IEnumerable<Category> existingCategories = await _categoryRepository.GetByUserIdAsync(userId);
        string categoryName = categoryDto.Name.Trim();
        
        //check for existing category
        if (existingCategories.Any(c => c.Id != categoryId && 
                                        string.Equals(categoryName, c.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateEntityException($"You have already created a category named {categoryDto.Name}.");
        }
        
        category.Name = categoryName;
        
        await _categoryRepository.UpdateAsync(category);
        return MapToDto(category);
    }

    private static CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name =  category.Name
        };
    }
}