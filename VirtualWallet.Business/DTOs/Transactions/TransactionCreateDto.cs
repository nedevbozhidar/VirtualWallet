using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Transactions;

public class TransactionCreateDto
{
    [Required(ErrorMessage = "Recipient username, email, or phone number is required.")]
    public string RecipientIdentifier { get; set; } = null!;

    [Range(0.01, 10000000, ErrorMessage = "Transfer amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public int SenderWalletId { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public int? CategoryId { get; set; }
}
