using VirtualWallet.Business.DTOs.Transactions;

namespace VirtualWallet.Business.Services.Contracts;

public interface ITransactionService
{
    Task<TransactionPreviewDto> PreviewTransactionAsync(int senderUserId, TransactionCreateDto createDto);
    Task<TransactionResponseDto> ExecuteTransactionAsync(int senderUserId, TransactionCreateDto createDto);
    Task<IEnumerable<TransactionResponseDto>> GetFilteredTransactionsAsync(TransactionFilterDto filterDto);
    Task<TransactionResponseDto> GetByIdAsync(int transactionId, int userId, bool isAdmin);
}
