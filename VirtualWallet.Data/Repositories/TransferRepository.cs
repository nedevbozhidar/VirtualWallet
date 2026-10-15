using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;
namespace VirtualWallet.Data.Repositories;

public class TransferRepository : ITransferRepository
{
    private readonly VirtualWalletDbContext _context;

    public TransferRepository(VirtualWalletDbContext context)
    {
        _context = context;
    }
    
    public async Task<Transfer?> GetByIdAsync(int id)
    {
        return await _context.Transfers
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transfer>> GetByWalletIdAsync(int walletId)
    {
        return await _context.Transfers
            .Where(t => t.WalletId == walletId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Transfer transfer)
    {
        await _context.Transfers.AddAsync(transfer);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}