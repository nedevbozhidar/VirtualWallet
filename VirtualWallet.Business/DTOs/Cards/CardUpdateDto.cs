using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Cards;

public class CardUpdateDto
{
    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Card holder must be between 2 and 30 symbols.")]
    public string CardHolder { get; set; } = null!;
}