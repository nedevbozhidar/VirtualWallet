namespace VirtualWallet.Business.DTOs.Users;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public string Role { get; set; } = null!;
    public bool IsBlocked { get; set; }
    public bool IsEmailVerified { get; set; }
    public string IdentityVerificationStatus { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
