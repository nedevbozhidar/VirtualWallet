using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly VirtualWalletDbContext _context;

    public UserRepository(VirtualWalletDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Wallets)
            .Include(u => u.Cards)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}