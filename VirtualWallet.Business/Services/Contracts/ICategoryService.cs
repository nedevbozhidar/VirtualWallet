using VirtualWallet.Business.DTOs.Categories;
using VirtualWallet.Data.Models;

namespace VirtualWallet.Business.Services.Contracts;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetUserCategoriesAsync(int userId);
    Task<CategoryResponseDto> CreateCategoryAsync(int userId, CategoryCreateDto categoryDto);
    Task<CategoryResponseDto> GetCategoryByIdAsync(int userId, int categoryId);
    Task<CategoryResponseDto> UpdateCategoryAsync(int userId, int categoryId, CategoryUpdateDto categoryDto);
}