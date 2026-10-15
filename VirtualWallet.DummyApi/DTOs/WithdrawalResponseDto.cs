namespace VirtualWallet.DummyApi.DTOs;

public class WithdrawalResponseDto
{
    public int Id { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
}