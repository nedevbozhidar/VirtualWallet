using System.Net.Http.Json;
using VirtualWallet.Business.DTOs.Transfers;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;
using VirtualWallet.Data.Security;

namespace VirtualWallet.Business.Services.Implementations;

public class TransferService : ITransferService
{
    private const decimal MaximumFundingAmount = 10_000_000m;

    private readonly ITransferRepository _transferRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly HttpClient _httpClient;
    private readonly ICardDataProtector _cardDataProtector;

    public TransferService(
        ITransferRepository transferRepository,
        ICardRepository cardRepository,
        IWalletRepository walletRepository,
        HttpClient httpClient,
        ICardDataProtector cardDataProtector)
    {
        _transferRepository = transferRepository;
        _cardRepository = cardRepository;
        _walletRepository = walletRepository;
        _httpClient = httpClient;
        _cardDataProtector = cardDataProtector;
    }
    
    public async Task<TransferResponseDto> FundWalletFromCardAsync(int userId, TransferCreateDto dto)
    {
        if (dto.Amount <= 0 || dto.Amount > MaximumFundingAmount)
        {
            throw new InvalidUserInputException(
                $"Funding amount must be greater than zero and no more than {MaximumFundingAmount:N0}.");
        }

        if (dto.CheckNumber is null
            || dto.CheckNumber.Length != 3
            || dto.CheckNumber.Any(character => !char.IsDigit(character)))
        {
            throw new InvalidUserInputException("Check number (CVV) must be exactly 3 digits.");
        }

        Card card = await _cardRepository.GetByIdAsync(dto.CardId)
                    ?? throw new EntityNotFoundException("Card was not found.");

        if (card.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not own this card.");
        }

        Wallet wallet = await _walletRepository.GetByIdAsync(dto.WalletId)
                        ?? throw new EntityNotFoundException("Wallet was not found.");

        if (wallet.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not own this wallet.");
        }

        var withdrawalRequest = new
        {
            CardNumber = _cardDataProtector.Unprotect(card.ProtectedCardNumber),
            CardHolder = card.CardHolder,
            ExpirationDate = card.ExpirationDate,
            CheckNumber = dto.CheckNumber,
            Amount = dto.Amount
        };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:5095/api/withdrawals", withdrawalRequest);
        var bankResult = await response.Content.ReadFromJsonAsync<WithdrawalResultDto>();

        bool isSuccess = bankResult?.IsSuccess ?? false;
        string message = bankResult?.Message ?? "Transaction was declined by card issuer.";

        if (isSuccess)
        {
            wallet.Balance += dto.Amount;
            await _walletRepository.UpdateAsync(wallet);
        }

        var transfer = new Transfer
        {
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow,
            CardId = card.Id,
            WalletId = wallet.Id,
            Status = isSuccess ? TransferStatus.Confirmed : TransferStatus.Declined
        };

        await _transferRepository.AddAsync(transfer);

        return new TransferResponseDto
        {
            TransferId = transfer.Id,
            Amount = transfer.Amount,
            Status = transfer.Status.ToString(),
            Message = message,
            CreatedAt = transfer.CreatedAt
        };
    }
    
    internal record WithdrawalResultDto(int Id, bool IsSuccess, string Message);
}
