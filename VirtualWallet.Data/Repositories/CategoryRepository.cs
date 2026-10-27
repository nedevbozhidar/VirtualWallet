using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly VirtualWalletDbContext _context;
    
    public CategoryRepository(VirtualWalletDbContext context)
    {
        _context = context;
    }
    
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId).AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
}