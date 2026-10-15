using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Wallets;

public class SavingsWalletCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    [Required]
    public int SourceWalletId { get; set; }

    [Range(1, 1000000, ErrorMessage = "Savings amount must be at least 1.")]
    public decimal Amount { get; set; }

    [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 months.")]
    public int DurationMonths { get; set; }
}
