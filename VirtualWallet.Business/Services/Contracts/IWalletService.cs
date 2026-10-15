using VirtualWallet.Business.DTOs.Wallets;

namespace VirtualWallet.Business.Services.Contracts;

public interface IWalletService
{
    Task<WalletResponseDto> CreateWalletAsync(int userId, WalletCreateDto walletDto);
    Task<IEnumerable<WalletResponseDto>> GetUserWalletsAsync(int userId);
    Task<WalletResponseDto> GetByIdAsync(int walletId, int userId);
    Task SetDefaultWalletAsync(int walletId, int userId);
    Task DeleteWalletAsync(int walletId, int userId);
    Task<WalletResponseDto> CreateSavingsWalletAsync(int userId, SavingsWalletCreateDto savingsDto);
}
