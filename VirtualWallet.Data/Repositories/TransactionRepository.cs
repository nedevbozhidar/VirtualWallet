using Microsoft.EntityFrameworkCore;
using VirtualWallet.Data.Context;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly VirtualWalletDbContext _context;

    public TransactionRepository(VirtualWalletDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Transactions
            .Include(t => t.SenderUser)
            .Include(t => t.SenderWallet)
                .ThenInclude(w => w.Currency)
            .Include(t => t.RecipientUser)
            .Include(t => t.RecipientWallet)
                .ThenInclude(w => w.Currency)
            .Include(t => t.Category)
            .Include(t => t.RecurringTransaction)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
    {
        return await _context.Transactions
            .Include(t => t.SenderUser)
            .Include(t => t.RecipientUser)
            .Include(t => t.Category)
            .Where(t => t.SenderWalletId == walletId || t.RecipientWalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
    {
        return await _context.Transactions
            .Include(t => t.SenderUser)
            .Include(t => t.RecipientUser)
            .Include(t => t.Category)
            .Where(t => t.SenderUserId == userId || t.RecipientUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
        int? walletId = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? senderUsername = null,
        string? recipientUsername = null,
        string? direction = null,
        string? sortBy = null,
        bool isDescending = true,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _context.Transactions
            .Include(t => t.SenderUser)
            .Include(t => t.SenderWallet)
            .Include(t => t.RecipientUser)
            .Include(t => t.RecipientWallet)
            .Include(t => t.Category)
            .AsQueryable();

        if (walletId.HasValue)
        {
            query = direction?.ToLower() switch
            {
                "incoming" => query.Where(t => t.RecipientWalletId == walletId),
                "outgoing" => query.Where(t => t.SenderWalletId == walletId),
                _ => query.Where(t => t.SenderWalletId == walletId || t.RecipientWalletId == walletId)
            };
        }

        if (userId.HasValue)
        {
            query = direction?.ToLower() switch
            {
                "incoming" => query.Where(t => t.RecipientUserId == userId),
                "outgoing" => query.Where(t => t.SenderUserId == userId),
                _ => query.Where(t => t.SenderUserId == userId || t.RecipientUserId == userId)
            };
        }

        if (startDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= endDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(senderUsername))
        {
            query = query.Where(t => t.SenderUser.Username == senderUsername);
        }

        if (!string.IsNullOrWhiteSpace(recipientUsername))
        {
            query = query.Where(t => t.RecipientUser.Username == recipientUsername);
        }

        query = sortBy?.ToLower() switch
        {
            "amount" => isDescending ? query.OrderByDescending(t => t.Amount) : query.OrderBy(t => t.Amount),
            _ => isDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task SaveTransactionAndWalletsAsync(Wallet senderWallet, Wallet recipientWallet, Transaction transaction)
    {
        _context.Entry(senderWallet)
            .Property(w => w.Balance).IsModified = true;
        _context.Entry(recipientWallet)
            .Property(w => w.Balance).IsModified = true;

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
}
