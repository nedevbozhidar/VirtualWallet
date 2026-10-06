using VirtualWallet.Data.Models.Enums;

namespace VirtualWallet.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public bool IsBlocked { get; set; }
    public bool IsEmailVerified { get; set; }

    // Identity Verification
    public IdentityVerificationStatus IdentityVerificationStatus { get; set; } = IdentityVerificationStatus.NotSubmitted;
    public string? IdCardPhotoUrl { get; set; }
    public string? SelfiePhotoUrl { get; set; }

    // Refer a Friend tracking
    public int ReferralsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public ICollection<Card> Cards { get; set; } = new List<Card>();
    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Contact> ContactOf { get; set; } = new List<Contact>();
    public ICollection<WalletMember> WalletMemberships { get; set; } = new List<WalletMember>();
    public ICollection<Referral> SentReferrals { get; set; } = new List<Referral>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
}
