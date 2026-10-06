using VirtualWallet.Data.Models.Enums;

namespace VirtualWallet.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Sender
    public int SenderUserId { get; set; }
    public User SenderUser { get; set; } = null!;
    public int SenderWalletId { get; set; }
    public Wallet SenderWallet { get; set; } = null!;

    // Recipient
    public int RecipientUserId { get; set; }
    public User RecipientUser { get; set; } = null!;
    public int RecipientWalletId { get; set; }
    public Wallet RecipientWallet { get; set; } = null!;

    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    // Spending Category (optional)
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Recurring Transaction link (if spawned by a recurring rule)
    public int? RecurringTransactionId { get; set; }
    public RecurringTransaction? RecurringTransaction { get; set; }

    // Large Transaction Verification
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiry { get; set; }
    public bool IsVerified { get; set; }

    // Currency exchange rate at time of transaction (for cross-currency)
    public decimal? ExchangeRate { get; set; }
}
