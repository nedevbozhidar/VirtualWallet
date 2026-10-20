using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Cards;

public class CardCreateDto
{
    [Required]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be exactly 16 digits.")]
    public string CardNumber { get; set; } = null!;

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Card holder must be between 2 and 30 symbols.")]
    public string CardHolder { get; set; } = null!;

}
