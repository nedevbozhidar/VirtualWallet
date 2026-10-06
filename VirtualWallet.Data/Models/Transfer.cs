using VirtualWallet.Data.Models.Enums;

namespace VirtualWallet.Data.Models;

public class Transfer
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CardId { get; set; }
    public Card Card { get; set; } = null!;

    public int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;

    public TransferStatus Status { get; set; } = TransferStatus.Pending;
}
