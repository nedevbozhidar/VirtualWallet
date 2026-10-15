namespace VirtualWallet.Business.DTOs.Wallets;

public class WalletResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Balance { get; set; }
    public bool IsDefault { get; set; }
    public string Type { get; set; } = null!;
    public int UserId { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public bool IsOverdraftEnabled { get; set; }
    public decimal OverdraftLimit { get; set; }
    public DateTime CreatedAt { get; set; }
}
