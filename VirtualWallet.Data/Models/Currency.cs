namespace VirtualWallet.Data.Models;

public class Currency
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;    // e.g., "EUR", "USD", "GBP"
    public string Name { get; set; } = null!;    // ex. American Dollar
    public decimal ExchangeRate { get; set; }     // Rate relative to base currency

    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
