using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Wallets;

public class WalletCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Wallet name must be between 2 and 50 characters.")]
    public string Name { get; set; } = null!;

    [Required]
    public int CurrencyId { get; set; }
}
