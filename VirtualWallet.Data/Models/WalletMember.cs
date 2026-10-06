namespace VirtualWallet.Data.Models;

public class WalletMember
{
    public int Id { get; set; }

    public int WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool CanSpend { get; set; }
    public bool CanAddMoney { get; set; }

    public DateTime AddedAt { get; set; }
}
