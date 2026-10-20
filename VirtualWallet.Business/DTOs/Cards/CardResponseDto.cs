namespace VirtualWallet.Business.DTOs.Cards;

public class CardResponseDto
{
    public int Id { get; set; }
    public string MaskedCardNumber { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public string CardHolder { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
