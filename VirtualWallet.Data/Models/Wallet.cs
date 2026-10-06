using VirtualWallet.Data.Models.Enums;

namespace VirtualWallet.Data.Models;

public class Wallet
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Balance { get; set; }
    public bool IsDefault { get; set; }
    public WalletType Type { get; set; } = WalletType.Personal;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;

    // Overdraft
    public bool IsOverdraftEnabled { get; set; }
    public decimal OverdraftLimit { get; set; }
    public int ConsecutiveNegativeMonths { get; set; }

    // Savings Wallet fields (nullable, only used when Type == Savings)
    public decimal? SavingsAmount { get; set; }
    public DateTime? SavingsMaturityDate { get; set; }
    public decimal? SavingsInterestRate { get; set; }
    public int? SourceWalletId { get; set; }
    public Wallet? SourceWallet { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public ICollection<WalletMember> Members { get; set; } = new List<WalletMember>();
    public ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
