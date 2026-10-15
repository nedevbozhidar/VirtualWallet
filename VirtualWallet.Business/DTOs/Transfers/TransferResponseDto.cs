namespace VirtualWallet.Business.DTOs.Transfers;

public class TransferResponseDto
{
    public int TransferId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!; //confirmed or declined
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
}