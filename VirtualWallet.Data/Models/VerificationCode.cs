namespace VirtualWallet.Data.Models;

public class VerificationCode
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>
    /// The type of verification: EmailVerification, LargeTransaction
    /// </summary>
    public string Type { get; set; } = null!;
    public string Code { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
}
