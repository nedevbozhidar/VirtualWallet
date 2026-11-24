using VirtualWallet.Business.DTOs.Users;

namespace VirtualWallet.Business.Services.Contracts;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto);
    Task<UserResponseDto> AuthenticateAsync(UserLoginDto loginDto);
    Task<UserResponseDto> GetByIdAsync(int id);
    Task<UserResponseDto> GetByUsernameAsync(string username);
    Task<UserResponseDto?> FindRecipientAsync(string? identifier);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto> UpdateProfileAsync(int userId, UserProfileUpdateDto updateDto);
    Task BlockUserAsync(int userId);
    Task UnblockUserAsync(int userId);
}
