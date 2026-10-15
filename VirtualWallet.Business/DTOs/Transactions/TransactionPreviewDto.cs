namespace VirtualWallet.Business.DTOs.Transactions;

public class TransactionPreviewDto
{
    public string RecipientUsername { get; set; } = null!;
    public decimal Amount { get; set; }
    public string SenderWalletName { get; set; } = null!;
    public string CurrencyCode { get; set; } = null!;
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
}
