using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.DummyApi.DTOs;

public class WithdrawalRequestDto
{
    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be exactly 16 digits.")]
    public string CardNumber { get; set; } = null!;

    [Required]
    [StringLength(30, MinimumLength = 2,ErrorMessage =  "Card holder must be between 2 and 30 characters long.")]
    public string CardHolder { get; set; } = null!;
    
    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Check number (CVV) must be exactly 3 digits.")]
    public string CheckNumber { get; set; } = null!;

    [Required] 
    [Range(0.01, 10000000)]
    public decimal Amount { get; set; }
}