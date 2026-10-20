using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Transfers;

public class TransferCreateDto
{
    public int CardId { get; set; }
    
    public int WalletId { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Check number (CVV) must be exactly 3 digits.")]
    public string CheckNumber { get; set; } = null!;
    
    [Range(0.01, 10000000, ErrorMessage = "Transfer amount must be greater than zero.")]
    public decimal Amount { get; set; }
}
