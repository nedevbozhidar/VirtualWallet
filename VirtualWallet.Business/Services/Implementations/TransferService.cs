using System.Net.Http.Json;
using VirtualWallet.Business.DTOs.Transfers;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Business.Services.Implementations;

public class TransferService : ITransferService
{
    private readonly ITransferRepository _transferRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly HttpClient _httpClient;

    public TransferService(
        ITransferRepository transferRepository,
        ICardRepository cardRepository,
        IWalletRepository walletRepository,
        HttpClient httpClient)
    {
        _transferRepository = transferRepository;
        _cardRepository = cardRepository;
        _walletRepository = walletRepository;
        _httpClient = httpClient;
    }
    
    public async Task<TransferResponseDto> FundWalletFromCardAsync(int userId, TransferCreateDto dto)
    {
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
            CardNumber = card.CardNumber,
            CardHolder = card.CardHolder,
            ExpirationDate = card.ExpirationDate,
            CheckNumber = card.CheckNumber,
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