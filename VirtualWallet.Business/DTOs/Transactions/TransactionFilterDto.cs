namespace VirtualWallet.Business.DTOs.Transactions;

public class TransactionFilterDto
{
    public int? WalletId { get; set; }
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SenderUsername { get; set; }
    public string? RecipientUsername { get; set; }
    public string? Direction { get; set; } // "incoming" or "outgoing"
    public string? SortBy { get; set; }    // "amount" or "date"
    public bool IsDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
