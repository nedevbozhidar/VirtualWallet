using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Users;

public class UserProfileUpdateDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string PhoneNumber { get; set; } = null!;

    public string? PhotoUrl { get; set; }
}
