using VirtualWallet.Data.Models;

namespace VirtualWallet.Data.Repositories.Contracts;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetByUserIdAsync(int userId);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
}