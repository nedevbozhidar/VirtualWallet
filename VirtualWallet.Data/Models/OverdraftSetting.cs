namespace VirtualWallet.Data.Models;

public class OverdraftSetting
{
    public int Id { get; set; }
    public decimal InterestRateModifier { get; set; }
    public DateTime EffectiveFrom { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; }
}
