using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualWallet.Data.Models;

namespace VirtualWallet.Data.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
        Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
            int? walletId = null,
            int? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? senderUsername = null,
            string? recipientUsername = null,
            string? sortBy = null,
            bool isDescending = true,
            int pageNumber = 1,
            int pageSize = 10);
        Task AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task SaveChangesAsync();
    }
}