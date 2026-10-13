using VirtualWallet.Data.Models;

namespace VirtualWallet.Data.Repositories.Contracts;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(int id);
    Task<IEnumerable<Card>> GetByUserIdAsync(int userId);
    Task<Card?> GetByCardNumberAsync(string cardNumber);
    Task<bool> ExistsByCardNumberAsync(string cardNumber);
    Task AddAsync(Card card);
    Task UpdateAsync(Card card);
    Task DeleteAsync(Card card);
    Task SaveChangesAsync();
}