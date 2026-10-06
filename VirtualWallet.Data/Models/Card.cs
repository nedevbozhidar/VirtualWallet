namespace VirtualWallet.Data.Models;

public class Card
{
    public int Id { get; set; }
    public string CardNumber { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public string CardHolder { get; set; } = null!;
    public string CheckNumber { get; set; } = null!;
    public bool IsPrimary { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
