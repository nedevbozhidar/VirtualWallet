using VirtualWallet.Business.DTOs.Wallets;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;
using VirtualWallet.Data.Models;
using VirtualWallet.Data.Models.Enums;
using VirtualWallet.Data.Repositories.Contracts;

namespace VirtualWallet.Business.Services.Implementations;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;

    public WalletService(IWalletRepository walletRepository, IUserRepository userRepository)
    {
        _walletRepository = walletRepository;
        _userRepository = userRepository;
    }

    public async Task<WalletResponseDto> CreateWalletAsync(int userId, WalletCreateDto walletDto)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new EntityNotFoundException($"User with ID {userId} was not found.");

        var existingWallets = await _walletRepository.GetByUserIdAsync(userId);
        var isFirstWallet = !existingWallets.Any();

        var wallet = new Wallet
        {
            Name = walletDto.Name,
            Balance = 0m,
            IsDefault = isFirstWallet,
            Type = WalletType.Personal,
            UserId = userId,
            CurrencyId = walletDto.CurrencyId,
            IsOverdraftEnabled = false,
            OverdraftLimit = 0m,
            CreatedAt = DateTime.UtcNow
        };

        await _walletRepository.AddAsync(wallet);

        // Reload with details for Currency mapping
        var created = await _walletRepository.GetByIdWithDetailsAsync(wallet.Id) ?? wallet;
        return MapToDto(created);
    }

    public async Task<IEnumerable<WalletResponseDto>> GetUserWalletsAsync(int userId)
    {
        var wallets = await _walletRepository.GetByUserIdAsync(userId);
        return wallets.Select(MapToDto);
    }

    public async Task<WalletResponseDto> GetByIdAsync(int walletId, int userId)
    {
        var wallet = await _walletRepository.GetByIdWithDetailsAsync(walletId)
                     ?? throw new EntityNotFoundException($"Wallet with ID {walletId} was not found.");

        if (wallet.UserId != userId && !wallet.Members.Any(m => m.UserId == userId))
        {
            throw new UnauthorizedOperationException("You do not have access to this wallet.");
        }

        return MapToDto(wallet);
    }

    public async Task SetDefaultWalletAsync(int walletId, int userId)
    {
        var targetWallet = await _walletRepository.GetByIdAsync(walletId)
                           ?? throw new EntityNotFoundException($"Wallet with ID {walletId} was not found.");

        if (targetWallet.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not own this wallet.");
        }

        var allWallets = await _walletRepository.GetByUserIdAsync(userId);
        foreach (var wallet in allWallets)
        {
            if (wallet.Id == walletId)
            {
                continue;
            }

            wallet.IsDefault = false;
            await _walletRepository.UpdateAsync(wallet);
        }

        targetWallet.IsDefault = true;
        await _walletRepository.UpdateAsync(targetWallet);
    }

    public async Task DeleteWalletAsync(int walletId, int userId)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId)
                     ?? throw new EntityNotFoundException($"Wallet with ID {walletId} was not found.");

        if (wallet.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not own this wallet.");
        }

        if (wallet.Balance > 0)
        {
            throw new InvalidUserInputException("Cannot delete a wallet with a remaining positive balance. Please transfer the funds first.");
        }

        var allWallets = (await _walletRepository.GetByUserIdAsync(userId)).ToList();
        if (allWallets.Count <= 1)
        {
            throw new InvalidUserInputException("You cannot delete your only wallet.");
        }

        if (wallet.IsDefault)
        {
            var nextWallet = allWallets.First(w => w.Id != walletId);
            nextWallet.IsDefault = true;
            await _walletRepository.UpdateAsync(nextWallet);
        }

        await _walletRepository.DeleteAsync(wallet);
    }

    public async Task<WalletResponseDto> CreateSavingsWalletAsync(int userId, SavingsWalletCreateDto savingsDto)
    {
        var sourceWallet = await _walletRepository.GetByIdAsync(savingsDto.SourceWalletId)
                           ?? throw new EntityNotFoundException($"Source wallet with ID {savingsDto.SourceWalletId} was not found.");

        if (sourceWallet.UserId != userId)
        {
            throw new UnauthorizedOperationException("You do not own the source wallet.");
        }

        if (sourceWallet.Balance < savingsDto.Amount)
        {
            throw new InsufficientFundsException("Insufficient funds in the source wallet to open this savings account.");
        }

        // Deduct from source wallet
        sourceWallet.Balance -= savingsDto.Amount;

        // Interest calculation: base 2.0% + 0.25% per each 6 months duration
        var interestRate = 0.02m + (savingsDto.DurationMonths / 6) * 0.0025m;

        var savingsWallet = new Wallet
        {
            Name = savingsDto.Name,
            Balance = savingsDto.Amount,
            IsDefault = false,
            Type = WalletType.Savings,
            UserId = userId,
            CurrencyId = sourceWallet.CurrencyId,
            SavingsAmount = savingsDto.Amount,
            SavingsInterestRate = interestRate,
            SavingsMaturityDate = DateTime.UtcNow.AddMonths(savingsDto.DurationMonths),
            SourceWalletId = sourceWallet.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _walletRepository.CreateSavingsWalletWithTransfersAsync(sourceWallet, savingsWallet);

        var created = await _walletRepository.GetByIdWithDetailsAsync(savingsWallet.Id) ?? savingsWallet;
        return MapToDto(created);
    }

    private static WalletResponseDto MapToDto(Wallet wallet)
    {
        return new WalletResponseDto
        {
            Id = wallet.Id,
            Name = wallet.Name,
            Balance = wallet.Balance,
            IsDefault = wallet.IsDefault,
            Type = wallet.Type.ToString(),
            UserId = wallet.UserId,
            CurrencyCode = wallet.Currency?.Code ?? "EUR",
            IsOverdraftEnabled = wallet.IsOverdraftEnabled,
            OverdraftLimit = wallet.OverdraftLimit,
            CreatedAt = wallet.CreatedAt
        };
    }
}
