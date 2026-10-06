using VirtualWallet.Data.Models.Enums;

namespace VirtualWallet.Data.Models;

public class RecurringTransaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }

    public int SenderWalletId { get; set; }
    public Wallet SenderWallet { get; set; } = null!;

    public int RecipientWalletId { get; set; }
    public Wallet RecipientWallet { get; set; } = null!;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public RecurringInterval Interval { get; set; }
    public DateTime NextExecutionDate { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
