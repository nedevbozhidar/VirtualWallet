using VirtualWallet.Business.DTOs.Transactions;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Business.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IUserRepository userRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _userRepository = userRepository;
    }

    public async Task<TransactionPreviewDto> PreviewTransactionAsync(int senderUserId, TransactionCreateDto createDto)
    {
        var (senderUser, senderWallet, recipientUser, recipientWallet) = await ValidateAndResolveParticipantsAsync(senderUserId, createDto);

        return new TransactionPreviewDto
        {
            RecipientUsername = recipientUser.Username,
            Amount = createDto.Amount,
            SenderWalletName = senderWallet.Name,
            CurrencyCode = senderWallet.Currency?.Code ?? "EUR",
            Description = createDto.Description
        };
    }

    public async Task<TransactionResponseDto> ExecuteTransactionAsync(int senderUserId, TransactionCreateDto createDto)
    {
        var (senderUser, senderWallet, recipientUser, recipientWallet) = await ValidateAndResolveParticipantsAsync(senderUserId, createDto);

        // Check available funds (including overdraft if enabled)
        var availableFunds = senderWallet.Balance + (senderWallet.IsOverdraftEnabled ? senderWallet.OverdraftLimit : 0m);
        if (availableFunds < createDto.Amount)
        {
            throw new InsufficientFundsException("Insufficient wallet balance to execute this transaction.");
        }

        // Cross-currency calculation
        decimal exchangeRate = 1.0m;
        decimal creditAmount = createDto.Amount;

        if (senderWallet.CurrencyId != recipientWallet.CurrencyId)
        {
            var senderRate = senderWallet.Currency?.ExchangeRate ?? 1.0m;
            var recipientRate = recipientWallet.Currency?.ExchangeRate ?? 1.0m;

            if (senderRate > 0)
            {
                exchangeRate = recipientRate / senderRate;
                creditAmount = Math.Round(createDto.Amount * exchangeRate, 2);
            }
        }

        // Adjust balances
        senderWallet.Balance -= createDto.Amount;
        recipientWallet.Balance += creditAmount;

        var transaction = new Transaction
        {
            Amount = createDto.Amount,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow,
            SenderUserId = senderUser.Id,
            SenderWalletId = senderWallet.Id,
            RecipientUserId = recipientUser.Id,
            RecipientWalletId = recipientWallet.Id,
            Status = TransactionStatus.Confirmed,
            CategoryId = createDto.CategoryId,
            ExchangeRate = senderWallet.CurrencyId != recipientWallet.CurrencyId ? exchangeRate : null,
            IsVerified = true
        };

        await _transactionRepository.SaveTransactionAndWalletsAsync(senderWallet, recipientWallet, transaction);

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            SenderUsername = senderUser.Username,
            RecipientUsername = recipientUser.Username,
            Status = transaction.Status.ToString(),
            ExchangeRate = transaction.ExchangeRate
        };
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetFilteredTransactionsAsync(TransactionFilterDto filterDto)
    {
        var transactions = await _transactionRepository.GetFilteredTransactionsAsync(
            walletId: filterDto.WalletId,
            userId: filterDto.UserId,
            startDate: filterDto.StartDate,
            endDate: filterDto.EndDate,
            senderUsername: filterDto.SenderUsername,
            recipientUsername: filterDto.RecipientUsername,
            direction: filterDto.Direction,
            sortBy: filterDto.SortBy,
            isDescending: filterDto.IsDescending,
            pageNumber: filterDto.PageNumber,
            pageSize: filterDto.PageSize);

        return transactions.Select(t => new TransactionResponseDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            SenderUsername = t.SenderUser?.Username ?? "Unknown",
            RecipientUsername = t.RecipientUser?.Username ?? "Unknown",
            Status = t.Status.ToString(),
            CategoryName = t.Category?.Name,
            ExchangeRate = t.ExchangeRate
        });
    }

    public async Task<TransactionResponseDto> GetByIdAsync(int transactionId, int userId, bool isAdmin)
    {
        var transaction = await _transactionRepository.GetByIdWithDetailsAsync(transactionId)
                          ?? throw new EntityNotFoundException($"Transaction with ID {transactionId} was not found.");

        if (!isAdmin && transaction.SenderUserId != userId && transaction.RecipientUserId != userId)
        {
            throw new UnauthorizedOperationException("You do not have access to view this transaction.");
        }

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            SenderUsername = transaction.SenderUser?.Username ?? "Unknown",
            RecipientUsername = transaction.RecipientUser?.Username ?? "Unknown",
            Status = transaction.Status.ToString(),
            CategoryName = transaction.Category?.Name,
            ExchangeRate = transaction.ExchangeRate
        };
    }

    private async Task<(User SenderUser, Wallet SenderWallet, User RecipientUser, Wallet RecipientWallet)>
        ValidateAndResolveParticipantsAsync(int senderUserId, TransactionCreateDto createDto)
    {
        var senderUser = await _userRepository.GetByIdAsync(senderUserId)
                         ?? throw new EntityNotFoundException($"Sender with ID {senderUserId} was not found.");

        if (senderUser.IsBlocked)
        {
            throw new BlockedAccountException("Your account is blocked. Blocked accounts cannot perform transactions.");
        }

        var senderWallet = await _walletRepository.GetByIdWithDetailsAsync(createDto.SenderWalletId)
                           ?? throw new EntityNotFoundException($"Sender wallet with ID {createDto.SenderWalletId} was not found.");

        if (senderWallet.UserId != senderUserId)
        {
            throw new UnauthorizedOperationException("You do not own the selected sender wallet.");
        }

        // Resolve recipient by username, email, or phone
        var identifier = createDto.RecipientIdentifier.Trim();
        var recipientUser = await _userRepository.GetByUsernameAsync(identifier)
                            ?? await _userRepository.GetByEmailAsync(identifier)
                            ?? await _userRepository.GetByPhoneNumberAsync(identifier)
                            ?? throw new EntityNotFoundException($"Recipient matching '{identifier}' was not found.");

        if (recipientUser.Id == senderUserId)
        {
            throw new InvalidUserInputException("You cannot send money to yourself.");
        }

        var recipientWallets = (await _walletRepository.GetByUserIdAsync(recipientUser.Id)).ToList();
        var recipientWallet = recipientWallets.FirstOrDefault(w => w.IsDefault)
                              ?? recipientWallets.FirstOrDefault()
                              ?? throw new EntityNotFoundException("Recipient does not have an active wallet to receive funds.");

        // Reload recipient wallet with currency details if necessary
        if (recipientWallet.Currency == null)
        {
            recipientWallet = await _walletRepository.GetByIdWithDetailsAsync(recipientWallet.Id) ?? recipientWallet;
        }

        return (senderUser, senderWallet, recipientUser, recipientWallet);
    }
}
