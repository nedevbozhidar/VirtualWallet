using VirtualWallet.Data.Models;

namespace VirtualWallet.Data.Repositories.Contracts;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(int id);
    Task<Wallet?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId);
    Task AddAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task DeleteAsync(Wallet wallet);
    Task SaveChangesAsync();
}