namespace VirtualWallet.Data.Models;

public class Referral
{
    public int Id { get; set; }

    public int InviterUserId { get; set; }
    public User InviterUser { get; set; } = null!;

    public string InviteeEmail { get; set; } = null!;
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public bool IsCompleted { get; set; }
    public decimal BonusAmount { get; set; }

    public DateTime CreatedAt { get; set; }
}
