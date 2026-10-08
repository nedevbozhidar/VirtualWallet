using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly VirtualWalletDbContext _context;

        public WalletRepository(VirtualWalletDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Wallet?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Wallets
                .Include(w => w.Currency)
                .Include(w => w.User)
                .Include(w => w.SourceWallet)
                .Include(w => w.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Wallet>> GetByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .Include(w => w.Currency)
                .Where(w => w.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}