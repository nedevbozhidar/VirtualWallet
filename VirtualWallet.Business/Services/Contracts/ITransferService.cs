using VirtualWallet.Business.DTOs.Transfers;
namespace VirtualWallet.Business.Services.Contracts;

public interface ITransferService
{
    Task<TransferResponseDto> FundWalletFromCardAsync(int userId, TransferCreateDto dto);
}