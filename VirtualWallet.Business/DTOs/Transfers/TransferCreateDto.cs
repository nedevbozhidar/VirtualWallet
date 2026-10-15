using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Transfers;

public class TransferCreateDto
{
    public int CardId { get; set; }
    
    public int WalletId { get; set; }
    
    [Range(0.01, 10000000, ErrorMessage = "Transfer amount must be greater than zero.")]
    public decimal Amount { get; set; }
}