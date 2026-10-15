using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Users;

public class UserRegisterDto
{
    [Required]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 20 symbols.")]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
