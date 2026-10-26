namespace VirtualWallet.Business.DTOs.Transfers;

public class TransferHistoryDto
{
    public int TransferId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
