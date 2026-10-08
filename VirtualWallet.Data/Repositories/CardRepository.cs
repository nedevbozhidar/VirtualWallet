using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Data.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly VirtualWalletDbContext _context;

        public CardRepository(VirtualWalletDbContext context)
        {
            _context = context;
        }

        public async Task<Card?> GetByIdAsync(int id)
        {
            return await _context.Cards
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Card>> GetByUserIdAsync(int userId)
        {
            return await _context.Cards
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Card?> GetByCardNumberAsync(string cardNumber)
        {
            return await _context.Cards
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }

        public async Task<bool> ExistsByCardNumberAsync(string cardNumber)
        {
            return await _context.Cards
                .AnyAsync(c => c.CardNumber == cardNumber);
        }

        public async Task AddAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Card card)
        {
            _context.Cards.Update(card);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Card card)
        {
            _context.Cards.Remove(card);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}