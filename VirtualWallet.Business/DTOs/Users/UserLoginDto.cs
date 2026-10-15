using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.Business.DTOs.Users;

public class UserLoginDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
