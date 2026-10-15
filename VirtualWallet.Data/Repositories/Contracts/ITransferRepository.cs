using VirtualWallet.Data.Models;

namespace VirtualWallet.Data.Repositories.Contracts;

public interface ITransferRepository
{
    Task<Transfer?> GetByIdAsync(int id);
    Task<IEnumerable<Transfer>> GetByWalletIdAsync(int walletId);
    Task AddAsync(Transfer transfer);
    Task SaveChangesAsync();
}