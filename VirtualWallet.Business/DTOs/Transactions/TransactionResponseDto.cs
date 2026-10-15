namespace VirtualWallet.Business.DTOs.Transactions;

public class TransactionResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SenderUsername { get; set; } = null!;
    public string RecipientUsername { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? CategoryName { get; set; }
    public decimal? ExchangeRate { get; set; }
}
